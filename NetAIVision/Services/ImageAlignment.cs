using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using OpenCvSharp;
using Size = OpenCvSharp.Size;

namespace NetAIVision.Services
{
    /// <summary>
    /// 优化后的图像对齐类（使用 Homography + 防抖/缓存 + 性能优化）
    /// 使用建议：
    /// 1. 程序启动时调用 InitTemplate(templateBitmap)
    /// 2. 每帧调用 AlignToTemplate(currentFrameBitmap)
    ///
    /// 可进一步优化：如果你能直接使用 Mat 而不是 Bitmap，请重载 AlignToTemplate 接收 Mat，避免 Bitmap->Mat 编解码开销。
    /// </summary>
    internal static class ImageAlignment
    {
        #region 可调参数

        // 每 N 帧重新计算一次 homography（减小抖动与计算量）
        public static int RecalcEveryNFrames = 3;

        // 用于筛选最优匹配的数量上限
        private static int MaxGoodMatches = 80;

        // BFMatcher 匹配点至少数量阈值
        private static int MinMatchesForHomography = 8;

        // 如果 newError 比 lastError 大于该倍率则拒绝更新（防止错误更新）
        private static double ErrorIncreaseTolerance = 1.2;

        // ORB 特征数量（可以根据场景增减）
        private static int OrbMaxFeatures = 2800;

        // RANSAC 重投影阈值（像素），可按场景微调
        private static double RansacReprojThreshold = 3.0;

        #endregion 可调参数

        private static Mat templateMatGray;
        private static Mat lastHomography;
        private static double lastAlignError = double.MaxValue;
        private static int frameCount = 0;

        // 持久化 ORB 与 Matcher，避免每帧重复分配
        private static ORB orb = ORB.Create(OrbMaxFeatures);

        private static BFMatcher bfMatcher = new BFMatcher(NormTypes.Hamming, crossCheck: true);

        private static readonly object syncLock = new object(); // 保证线程安全（如果多线程调用 AlignToTemplate）

        /// <summary>
        /// 初始化模板，只需调用一次
        /// </summary>
        public static void InitTemplate(Bitmap templateBitmap)
        {
            if (templateBitmap == null) throw new ArgumentNullException(nameof(templateBitmap));

            // 若已有 templateMatGray，则释放旧资源
            lock (syncLock)
            {
                templateMatGray?.Dispose();
                templateMatGray = BitmapToMat(templateBitmap);
                Cv2.CvtColor(templateMatGray, templateMatGray, ColorConversionCodes.BGR2GRAY);

                // 重置状态
                lastHomography?.Dispose();
                lastHomography = null;
                lastAlignError = double.MaxValue;
                frameCount = 0;
            }
        }

        /// <summary>
        /// 每帧对齐到模板（含抖动抑制、性能优化）
        /// 注意：入参为 Bitmap（如果可以，改为直接传 Mat 更高效）
        /// </summary>
        public static Bitmap AlignToTemplate(Bitmap srcBitmap, Rect? optionalRoi = null)
        {
            if (srcBitmap == null || templateMatGray == null)
                return srcBitmap;

            // 将 Bitmap 转 Mat（若可以直接拿 Mat，请写另一个重载以减少开销）
            Mat srcMat = null;
            Mat srcGray = null;
            Mat aligned = null;

            lock (syncLock) // 保护全局 state（lastHomography 等）
            {
                try
                {
                    srcMat = BitmapToMat(srcBitmap);

                    // 如果模板尺寸与输入差距很大，可先缩放到模板相同大小（可选，注释掉按需）
                    // Cv2.Resize(srcMat, srcMat, new Size(templateMatGray.Width, templateMatGray.Height));

                    srcGray = new Mat();
                    Cv2.CvtColor(srcMat, srcGray, ColorConversionCodes.BGR2GRAY);

                    frameCount++;
                    bool needRecalc = (frameCount % RecalcEveryNFrames == 0) || (lastHomography == null);

                    Mat H = lastHomography;

                    if (needRecalc)
                    {
                        // 如果提供 ROI，截取 ROI（在 srcGray 坐标系）
                        Mat srcForMatch = srcGray;
                        if (optionalRoi.HasValue)
                        {
                            var r = optionalRoi.Value;
                            // 确保 ROI 在范围内
                            r.X = Math.Max(0, Math.Min(r.X, srcGray.Width - 1));
                            r.Y = Math.Max(0, Math.Min(r.Y, srcGray.Height - 1));
                            r.Width = Math.Max(1, Math.Min(r.Width, srcGray.Width - r.X));
                            r.Height = Math.Max(1, Math.Min(r.Height, srcGray.Height - r.Y));
                            srcForMatch = new Mat(srcGray, new OpenCvSharp.Rect(r.X, r.Y, r.Width, r.Height));
                        }

                        // 计算特征点与描述子（模板只需计算一次，但 ORB.DetectAndCompute 需要输入每次）
                        KeyPoint[] kpTemplate;
                        Mat desTemplate = new Mat();
                        orb.DetectAndCompute(templateMatGray, null, out kpTemplate, desTemplate);

                        KeyPoint[] kpSrc;
                        Mat desSrc = new Mat();
                        orb.DetectAndCompute(srcForMatch, null, out kpSrc, desSrc);

                        // 若使用了 ROI，kpSrc 中点坐标需要偏移回整个 srcGray 坐标系
                        if (optionalRoi.HasValue && kpSrc != null && kpSrc.Length > 0)
                        {
                            var r = optionalRoi.Value;
                            for (int i = 0; i < kpSrc.Length; i++)
                            {
                                kpSrc[i].Pt = new Point2f(kpSrc[i].Pt.X + r.X, kpSrc[i].Pt.Y + r.Y);
                            }
                        }

                        // 验证描述子
                        if (desTemplate.Empty() || desSrc.Empty())
                        {
                            desTemplate?.Dispose();
                            desSrc?.Dispose();
                            if (srcForMatch != srcGray) srcForMatch.Dispose();
                            return srcBitmap;
                        }

                        // 匹配
                        var matches = bfMatcher.Match(desTemplate, desSrc);
                        if (matches == null || matches.Length < MinMatchesForHomography)
                        {
                            desTemplate.Dispose();
                            desSrc.Dispose();
                            if (srcForMatch != srcGray) srcForMatch.Dispose();
                            return srcBitmap;
                        }

                        // 取最优匹配
                        Array.Sort(matches, (a, b) => a.Distance.CompareTo(b.Distance));
                        var goodMatches = matches.Take(Math.Min(MaxGoodMatches, matches.Length)).ToArray();

                        var ptsTemplate = goodMatches.Select(m => kpTemplate[m.QueryIdx].Pt).ToArray();
                        var ptsSrc = goodMatches.Select(m => kpSrc[m.TrainIdx].Pt).ToArray();

                        // 计算 Homography（RANSAC）
                        Mat newH = Cv2.FindHomography(InputArray.Create(ptsSrc), InputArray.Create(ptsTemplate),
                                                      HomographyMethods.Ransac, RansacReprojThreshold);

                        // 计算误差（平均像素误差）
                        double error = ComputeAlignmentError(ptsTemplate, ptsSrc, newH);

                        // 如果没有历史矩阵或误差满足阈值（允许小幅变差），则更新
                        if (lastHomography == null || (error < lastAlignError * ErrorIncreaseTolerance))
                        {
                            lastHomography?.Dispose();
                            lastHomography = newH; // 采用新矩阵
                            lastAlignError = error;
                            H = newH;
                        }
                        else
                        {
                            // 否则丢弃 newH，保留旧矩阵
                            newH.Dispose();
                        }

                        // 清理
                        desTemplate.Dispose();
                        desSrc.Dispose();
                        if (srcForMatch != srcGray) srcForMatch.Dispose();
                    }

                    if (H == null || H.Empty())
                    {
                        return srcBitmap;
                    }

                    // 使用 Homography 做透视变换对齐
                    aligned = new Mat();
                    Cv2.WarpPerspective(srcMat, aligned, H, new Size(templateMatGray.Width, templateMatGray.Height));
                    return MatToBitmap(aligned);
                }
                finally
                {
                    // 释放临时 Mat（注意：lastHomography 与 templateMatGray 由全局持有，不在此释放）
                    srcGray?.Dispose();
                    srcMat?.Dispose();
                    aligned?.Dispose(); // MatToBitmap 已复制数据，安全释放
                }
            }
        }

        /// <summary>
        /// 计算对齐误差（平均重投影误差）
        /// </summary>
        private static double ComputeAlignmentError(Point2f[] templatePts, Point2f[] srcPts, Mat H)
        {
            if (H == null || H.Empty() || templatePts == null || templatePts.Length == 0)
                return double.MaxValue;

            try
            {
                Point2f[] projected = Cv2.PerspectiveTransform(srcPts, H);
                double total = 0;
                int n = Math.Min(projected.Length, templatePts.Length);
                for (int i = 0; i < n; i++)
                {
                    double dx = projected[i].X - templatePts[i].X;
                    double dy = projected[i].Y - templatePts[i].Y;
                    total += Math.Sqrt(dx * dx + dy * dy);
                }
                return total / n;
            }
            catch
            {
                return double.MaxValue;
            }
        }

        #region Bitmap <-> Mat (优化)

        /// <summary>
        /// 将 Bitmap 解码为 Mat（使用内存流，兼容性高）。
        /// 若性能成为瓶颈，请改为直接从相机 SDK 获取 byte[] 并使用 Cv2.ImDecode 或 Cv2.CvtColor/Cv2.Merge 构造 Mat。
        /// </summary>
        public static Mat BitmapToMat(Bitmap bitmap)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                byte[] bytes = ms.ToArray();
                return Cv2.ImDecode(bytes, ImreadModes.Color);
            }
        }

        /// <summary>
        /// 将 Mat 转为 Bitmap（返回新 Bitmap 副本）
        /// </summary>
        public static Bitmap MatToBitmap(Mat mat)
        {
            if (mat == null || mat.Empty())
                return null;

            Bitmap bmp;

            if (mat.Channels() == 1)
            {
                // 灰度图
                bmp = new Bitmap(mat.Width, mat.Height, PixelFormat.Format8bppIndexed);
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.WriteOnly, bmp.PixelFormat);

                int length = mat.Width * mat.Height;
                byte[] buffer = new byte[length];
                Marshal.Copy(mat.Data, buffer, 0, length);
                Marshal.Copy(buffer, 0, data.Scan0, length);
                bmp.UnlockBits(data);

                // 设置灰度调色板
                ColorPalette palette = bmp.Palette;
                for (int i = 0; i < 256; i++)
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                bmp.Palette = palette;
            }
            else if (mat.Channels() == 3)
            {
                // BGR -> RGB
                using (var rgbMat = new Mat())
                {
                    Cv2.CvtColor(mat, rgbMat, ColorConversionCodes.BGR2RGB);
                    bmp = new Bitmap(rgbMat.Width, rgbMat.Height, PixelFormat.Format24bppRgb);
                    BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                        ImageLockMode.WriteOnly, bmp.PixelFormat);

                    int length = (int)rgbMat.Step() * rgbMat.Rows;
                    byte[] buffer = new byte[length];
                    Marshal.Copy(rgbMat.Data, buffer, 0, length);
                    Marshal.Copy(buffer, 0, data.Scan0, length);
                    bmp.UnlockBits(data);
                }
            }
            else
            {
                throw new NotSupportedException("Unsupported Mat format: channels=" + mat.Channels());
            }

            return new Bitmap(bmp); // 返回安全副本
        }

        #endregion Bitmap <-> Mat (优化)
    }
}