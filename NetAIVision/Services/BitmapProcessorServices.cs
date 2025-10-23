using MvCameraControl;
using OpenCvSharp;
using OpenCvSharp.Features2D;
using OpenCvSharp.Flann;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tesseract;
using ZXing;
using ZXing.Common;

namespace NetAIVision.Services
{
    public class BitmapProcessorServices
    {
        //OCR
        private static string _lang = "chi_sim"; // 可改为 eng, chi_tra 等

        public static string _tessDataPath;

        public BitmapProcessorServices()
        {
        }

        #region 像素级处理辅助方法

        /// <summary>
        /// 对图像每个像素应用颜色转换函数（使用 LockBits 提高性能）
        /// </summary>
        private static Bitmap ProcessPixelByPixel(Bitmap src, Func<Color, Color> transform)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));

            Bitmap dst = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(0, 0, src.Width, src.Height);

            BitmapData srcData = src.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData dstData = dst.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* srcPtr = (byte*)srcData.Scan0;
                byte* dstPtr = (byte*)dstData.Scan0;

                int stride = srcData.Stride;
                int bytesPerPixel = 4;

                for (int y = 0; y < src.Height; y++)
                {
                    byte* rowSrc = srcPtr + y * stride;
                    byte* rowDst = dstPtr + y * stride;

                    for (int x = 0; x < src.Width; x++)
                    {
                        int idx = x * bytesPerPixel;
                        Color c = Color.FromArgb(
                            rowSrc[idx + 3],
                            rowSrc[idx + 2],
                            rowSrc[idx + 1],
                            rowSrc[idx + 0]
                        );

                        Color nc = transform(c);

                        rowDst[idx + 3] = nc.A;
                        rowDst[idx + 2] = nc.R;
                        rowDst[idx + 1] = nc.G;
                        rowDst[idx + 0] = nc.B;
                    }
                }
            }

            src.UnlockBits(srcData);
            dst.UnlockBits(dstData);

            return dst;
        }

        #endregion 像素级处理辅助方法

        #region 图像处理方法

        /// <summary>
        /// 反色（负片效果）
        /// </summary>
        public static Bitmap Invert(Bitmap bitmap)
        {
            return ProcessPixelByPixel(bitmap, c => Color.FromArgb(c.A, 255 - c.R, 255 - c.G, 255 - c.B));
        }

        /// <summary>
        /// OCR
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static string OCRFn(Bitmap bitmap)
        {
            // 使用 Tesseract 进行 OCR 识别
            var engine = new TesseractEngine(_tessDataPath, _lang, EngineMode.Default);
            // 将 Bitmap 转为 Pix（内存中完成，不保存文件）
            var ms = new MemoryStream();
            bitmap = PreprocessForOCR(bitmap);
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Tiff); // 推荐 TIFF，支持灰度/二值化
            ms.Position = 0;

            var img = Pix.LoadFromMemory(ms.ToArray());
            //var page = engine.Process(img);
            //string text = page.GetText();
            //return text;
            using (var page = engine.Process(img))
            {
                string text = page.GetText();

                // 清理文本：去除所有换行、回车、制表符，并压缩空白
                string cleaned = Regex.Replace(text, @"[\r\n\t\f]+", " ", RegexOptions.None);
                cleaned = Regex.Replace(cleaned, @"\s+", " "); // 多个空格合并为一个
                cleaned = cleaned.Trim();

                return cleaned;
            }
        }

        public static Bitmap PreprocessForOCR(Bitmap src)
        {
            var gray = new Bitmap(src.Width, src.Height, PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(gray))
            {
                g.DrawImage(src, 0, 0, src.Width, src.Height);
            }

            // 转灰度
            for (int y = 0; y < gray.Height; y++)
            {
                for (int x = 0; x < gray.Width; x++)
                {
                    Color c = gray.GetPixel(x, y);
                    int l = (int)(0.299 * c.R + 0.587 * c.G + 0.114 * c.B);
                    gray.SetPixel(x, y, Color.FromArgb(l, l, l));
                }
            }

            return gray;
        }

        /// <summary>
        /// QR code 解码
        /// </summary>
        /// <param name="roiImage"></param>
        /// <returns></returns>
        public static (bool flag, string txt, string message) QR_Code(Bitmap roiImage)
        {
            var barcodeReader = new ZXing.BarcodeReader()
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = new List<BarcodeFormat> {
                        BarcodeFormat.QR_CODE,
                        ZXing.BarcodeFormat.DATA_MATRIX,
                        ZXing.BarcodeFormat.AZTEC,
                        ZXing.BarcodeFormat.PDF_417
                    }
                }
            };
            var result = barcodeReader.Decode(roiImage);

            // 4. 显示结果
            if (result != null)
            {
                return (true, result.Text, $"识别成功：{result.Text}");
                //logHelper.AppendLog($"INFO:{selectedRoi.Name} 识别成功：{result.Text}");
                //MessageBox.Show($"识别成功：\n{result.Text}", "二维码内容", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                return (false, result.Text, $"未识别到二维码，请确保该区域包含清晰的二维码。");
                //logHelper.AppendLog($"ERROR:{selectedRoi.Name} 未识别到二维码，请确保该区域包含清晰的二维码");
                //MessageBox.Show("未识别到二维码，请确保该区域包含清晰的二维码。", "识别失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 调整亮度（-255 到 255）
        /// </summary>
        public static Bitmap AdjustBrightness(Bitmap bitmap, int brightness)
        {
            //byte b = (byte)Math.Abs(brightness);
            return ProcessPixelByPixel(bitmap, c =>
            {
                int r = c.R + brightness;
                int g = c.G + brightness;
                int b = c.B + brightness;

                r = Math.Max(0, Math.Min(255, r));
                g = Math.Max(0, Math.Min(255, g));
                b = Math.Max(0, Math.Min(255, b));

                return Color.FromArgb(c.A, (byte)r, (byte)g, (byte)b);
            });
        }

        /// <summary>
        /// 转为灰度图（加权平均法）
        /// </summary>
        public static Bitmap ToGrayscale(Bitmap bitmap)
        {
            return ProcessPixelByPixel(bitmap, c =>
            {
                int gray = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                return Color.FromArgb(c.A, gray, gray, gray);
            });
        }

        /// <summary>
        /// 二值化（基于阈值）
        /// </summary>
        public static Bitmap Threshold(Bitmap bitmap, byte threshold = 128)
        {
            return ProcessPixelByPixel(bitmap, c =>
            {
                int gray = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                byte v = gray >= threshold ? (byte)255 : (byte)0;
                return Color.FromArgb(c.A, v, v, v);
            });
        }

        /// <summary>
        /// 水平翻转
        /// </summary>
        public static Bitmap FlipHorizontal(Bitmap bitmap)
        {
            Bitmap result = new Bitmap(bitmap.Width, bitmap.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.ScaleTransform(-1, 1);
                g.TranslateTransform(-bitmap.Width, 0);
                g.DrawImage(bitmap, new System.Drawing.Point(0, 0));
            }
            return result;
        }

        /// <summary>
        /// 垂直翻转
        /// </summary>
        public static Bitmap FlipVertical(Bitmap bitmap)
        {
            Bitmap result = new Bitmap(bitmap.Width, bitmap.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.ScaleTransform(1, -1);
                g.TranslateTransform(0, -bitmap.Height);
                g.DrawImage(bitmap, new System.Drawing.Point(0, 0));
            }
            return result;
        }

        #region 图像降噪处理

        /// <summary>
        /// 中值滤波（有效去除椒盐噪声、脏点）
        /// </summary>
        /// <param name="bitmap">输入图像</param>
        /// <param name="kernelSize">滤波窗口大小（建议 3 或 5，必须为奇数）param>
        /// <returns>去噪后的图像</returns>
        public static Bitmap MedianBlur(Bitmap bitmap, int kernelSize = 3)
        {
            if (kernelSize % 2 == 0) kernelSize++; // 必须为奇数
            int radius = kernelSize / 2;

            Bitmap result = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData resultData = result.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                byte* resultPtr = (byte*)resultData.Scan0;
                int stride = data.Stride;
                int bytesPerPixel = 4;

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        List<int> rList = new List<int>(), gList = new List<int>(), bList = new List<int>();

                        // 收集邻域像素
                        for (int ky = -radius; ky <= radius; ky++)
                        {
                            for (int kx = -radius; kx <= radius; kx++)
                            {
                                int nx = x + kx;
                                int ny = y + ky;

                                if (nx >= 0 && nx < bitmap.Width && ny >= 0 && ny < bitmap.Height)
                                {
                                    int idx = (ny * stride) + (nx * bytesPerPixel);
                                    bList.Add(ptr[idx + 0]);
                                    gList.Add(ptr[idx + 1]);
                                    rList.Add(ptr[idx + 2]);
                                }
                            }
                        }

                        // 排序取中值
                        int dstIdx = (y * stride) + (x * bytesPerPixel);
                        resultPtr[dstIdx + 0] = GetMedian(bList);
                        resultPtr[dstIdx + 1] = GetMedian(gList);
                        resultPtr[dstIdx + 2] = GetMedian(rList);
                        resultPtr[dstIdx + 3] = 255; // Alpha
                    }
                }
            }

            bitmap.UnlockBits(data);
            result.UnlockBits(resultData);

            return result;
        }

        /// <summary>
        /// 均值滤波（简单平滑，轻微去噪）
        /// </summary>
        /// <param name="bitmap">输入图像</param>
        /// <param name="kernelSize">窗口大小（3x3, 5x5）</param>
        /// <returns>平滑后的图像</returns>
        public static Bitmap MeanBlur(Bitmap bitmap, int kernelSize = 3)
        {
            if (kernelSize % 2 == 0) kernelSize++;
            int radius = kernelSize / 2;

            Bitmap result = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData resultData = result.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                byte* resultPtr = (byte*)resultData.Scan0;
                int stride = data.Stride;
                int bytesPerPixel = 4;

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        int r = 0, g = 0, b = 0, count = 0;

                        for (int ky = -radius; ky <= radius; ky++)
                        {
                            for (int kx = -radius; kx <= radius; kx++)
                            {
                                int nx = x + kx;
                                int ny = y + ky;

                                if (nx >= 0 && nx < bitmap.Width && ny >= 0 && ny < bitmap.Height)
                                {
                                    int idx = (ny * stride) + (nx * bytesPerPixel);
                                    b += ptr[idx + 0];
                                    g += ptr[idx + 1];
                                    r += ptr[idx + 2];
                                    count++;
                                }
                            }
                        }

                        int dstIdx = (y * stride) + (x * bytesPerPixel);
                        resultPtr[dstIdx + 0] = (byte)(b / count);
                        resultPtr[dstIdx + 1] = (byte)(g / count);
                        resultPtr[dstIdx + 2] = (byte)(r / count);
                        resultPtr[dstIdx + 3] = 255;
                    }
                }
            }

            bitmap.UnlockBits(data);
            result.UnlockBits(resultData);

            return result;
        }

        /// <summary>
        /// 双边滤波（保边去噪，效果好但较慢）
        /// </summary>
        /// <param name="bitmap">输入图像</param>
        /// <param name="d">邻域直径（建议 5）</param>
        /// <param name="sigmaColor">颜色相似性权重（越大越平滑）</param>
        /// <param name="sigmaSpace">空间距离权重</param>
        /// <returns>去噪图像</returns>
        public static Bitmap BilateralFilter(Bitmap bitmap, int d = 5, double sigmaColor = 75, double sigmaSpace = 75)
        {
            int radius = d / 2;
            Bitmap result = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData resultData = result.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                byte* resultPtr = (byte*)resultData.Scan0;
                int stride = data.Stride;
                int bytesPerPixel = 4;

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        double rSum = 0, gSum = 0, bSum = 0, weightSum = 0;

                        for (int ky = -radius; ky <= radius; ky++)
                        {
                            for (int kx = -radius; kx <= radius; kx++)
                            {
                                int nx = x + kx;
                                int ny = y + ky;

                                if (nx >= 0 && nx < bitmap.Width && ny >= 0 && ny < bitmap.Height)
                                {
                                    int idx = (ny * stride) + (nx * bytesPerPixel);
                                    int idx2 = (y * stride) + (x * bytesPerPixel);

                                    double spatial = Math.Sqrt(kx * kx + ky * ky);
                                    double colorDiff = Distance(ptr[idx + 0], ptr[idx + 1], ptr[idx + 2],
                                                                ptr[idx2 + 0], ptr[idx2 + 1], ptr[idx2 + 2]);
                                    double weight = Math.Exp(-spatial / sigmaSpace) * Math.Exp(-colorDiff / sigmaColor);

                                    bSum += ptr[idx + 0] * weight;
                                    gSum += ptr[idx + 1] * weight;
                                    rSum += ptr[idx + 2] * weight;
                                    weightSum += weight;
                                }
                            }
                        }

                        int dstIdx = (y * stride) + (x * bytesPerPixel);
                        resultPtr[dstIdx + 0] = (byte)(bSum / weightSum);
                        resultPtr[dstIdx + 1] = (byte)(gSum / weightSum);
                        resultPtr[dstIdx + 2] = (byte)(rSum / weightSum);
                        resultPtr[dstIdx + 3] = 255;
                    }
                }
            }

            bitmap.UnlockBits(data);
            result.UnlockBits(resultData);

            return result;
        }

        // 辅助函数：计算中值
        private static byte GetMedian(List<int> values)
        {
            var sorted = values.OrderBy(x => x).ToList();
            int mid = sorted.Count / 2;
            return (byte)(sorted.Count % 2 == 0 ? (sorted[mid - 1] + sorted[mid]) / 2 : sorted[mid]);
        }

        // 辅助函数：计算颜色距离
        private static double Distance(int b1, int g1, int r1, int b2, int g2, int r2)
        {
            return (r1 - r2) * (r1 - r2) + (g1 - g2) * (g1 - g2) + (b1 - b2) * (b1 - b2);
        }

        #endregion 图像降噪处理

        /// <summary>
        /// 提升图像清晰度（锐化 + 高质量缩放）
        /// </summary>
        /// <param name="bitmap">原始图像</param>
        /// <param name="scaleFactor">放大倍数（如 2.0 表示放大 2 倍）</param>
        /// <returns>更清晰的图像</returns>
        public static Bitmap EnhanceSharpness(Bitmap bitmap, double scaleFactor = 1.0)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            if (scaleFactor <= 0) throw new ArgumentException("scaleFactor 必须大于 0");

            // 步骤1：先缩放到目标尺寸（高质量插值）
            int newWidth = (int)(bitmap.Width * scaleFactor);
            int newHeight = (int)(bitmap.Height * scaleFactor);

            Bitmap resized = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(resized))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(bitmap, new Rectangle(0, 0, newWidth, newHeight));
            }

            // 步骤2：应用锐化滤波
            return ApplySharpen(resized);
        }

        /// <summary>
        /// 应用锐化卷积核
        /// </summary>
        private static Bitmap ApplySharpen(Bitmap bitmap)
        {
            // 锐化卷积核（强调中心像素）
            int[,] kernel = {
        { 0, -1,  0 },
        { -1, 5, -1 },
        { 0, -1,  0 }
    };
            int factor = 1;
            int bias = 0;

            return Convolve(bitmap, kernel, factor, bias);
        }

        /// <summary>
        /// 卷积操作（用于模糊、锐化等）
        /// </summary>
        private static Bitmap Convolve(Bitmap bitmap, int[,] kernel, int factor = 1, int bias = 0)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData resultData = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                byte* resultPtr = (byte*)resultData.Scan0;
                int stride = data.Stride;

                int radius = kernel.GetLength(0) / 2;

                for (int y = radius; y < height - radius; y++)
                {
                    for (int x = radius; x < width - radius; x++)
                    {
                        double r = 0, g = 0, b = 0;

                        for (int ky = -radius; ky <= radius; ky++)
                        {
                            for (int kx = -radius; kx <= radius; kx++)
                            {
                                int nx = x + kx;
                                int ny = y + ky;
                                int idx = (ny * stride) + (nx * 4);
                                int w = kernel[ky + radius, kx + radius];

                                b += ptr[idx + 0] * w;
                                g += ptr[idx + 1] * w;
                                r += ptr[idx + 2] * w;
                            }
                        }

                        int dstIdx = (y * stride) + (x * 4);
                        resultPtr[dstIdx + 0] = Clamp((int)(b / factor) + bias);
                        resultPtr[dstIdx + 1] = Clamp((int)(g / factor) + bias);
                        resultPtr[dstIdx + 2] = Clamp((int)(r / factor) + bias);
                        resultPtr[dstIdx + 3] = 255;
                    }
                }
            }

            bitmap.UnlockBits(data);
            result.UnlockBits(resultData);

            return result;
        }

        // 辅助函数：限制值在 [0,255]
        private static byte Clamp(int value) => (byte)(value < 0 ? 0 : (value > 255 ? 255 : value));

        #region 边缘检测 - Sobel 算子

        /// <summary>
        /// 使用 Sobel 算子检测图像边缘
        /// </summary>
        /// <param name="bitmap">输入图像（建议先转为灰度图效果更好）</param>
        /// <returns>边缘图（灰度图）</returns>
        public static Bitmap DetectEdges(Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));

            // 转为灰度图（如果还不是）
            Bitmap gray = bitmap.PixelFormat == PixelFormat.Format8bppIndexed ? bitmap : ToGrayscale(bitmap);

            int width = gray.Width;
            int height = gray.Height;
            Bitmap edgeMap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            // Sobel 算子卷积核
            int[,] sobelX = {
        { -1, 0, 1 },
        { -2, 0, 2 },
        { -1, 0, 1 }
    };

            int[,] sobelY = {
        { -1, -2, -1 },
        {  0,  0,  0 },
        {  1,  2,  1 }
    };

            BitmapData grayData = gray.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData edgeData = edgeMap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* grayPtr = (byte*)grayData.Scan0;
                byte* edgePtr = (byte*)edgeData.Scan0;
                int stride = grayData.Stride;

                // 遍历每个像素（跳过边界）
                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        double gx = 0, gy = 0;

                        // 3x3 卷积
                        for (int ky = -1; ky <= 1; ky++)
                        {
                            for (int kx = -1; kx <= 1; kx++)
                            {
                                int nx = x + kx;
                                int ny = y + ky;
                                int idx = (ny * stride) + (nx * 4); // 假设是 Format32bppArgb
                                byte pixel = grayPtr[idx]; // 取蓝通道（灰度图 RGB 相同）

                                gx += pixel * sobelX[ky + 1, kx + 1];
                                gy += pixel * sobelY[ky + 1, kx + 1];
                            }
                        }

                        // 计算梯度幅值
                        int magnitude = (int)Math.Sqrt(gx * gx + gy * gy);
                        magnitude = Math.Min(255, Math.Max(0, magnitude));

                        // 写入结果
                        int dstIdx = (y * stride) + (x * 4);
                        edgePtr[dstIdx + 0] = (byte)magnitude; // B
                        edgePtr[dstIdx + 1] = (byte)magnitude; // G
                        edgePtr[dstIdx + 2] = (byte)magnitude; // R
                        edgePtr[dstIdx + 3] = 255;            // A
                    }
                }
            }

            gray.UnlockBits(grayData);
            edgeMap.UnlockBits(edgeData);

            return edgeMap;
        }

        /// <summary>
        /// 检测边缘并返回彩色边缘图（基于原图颜色强度）
        /// </summary>
        /// <param name="original">原图</param>
        /// <returns>彩色边缘图</returns>
        public static Bitmap DetectEdgesColored(Bitmap original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));

            Bitmap gray = ToGrayscale(original);
            Bitmap edges = DetectEdges(gray); // 获取灰度边缘

            Bitmap result = new Bitmap(original.Width, original.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                // 将边缘图作为遮罩，与原图融合
                using (ImageAttributes attrs = new ImageAttributes())
                {
                    ColorMap[] remap = {
                new ColorMap { OldColor = Color.Black, NewColor = Color.Transparent },
                new ColorMap { OldColor = Color.White, NewColor = Color.Red } // 可改为其他颜色
            };
                    attrs.SetRemapTable(remap, ColorAdjustType.Bitmap);

                    Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);
                    g.DrawImage(original, rect);
                    g.DrawImage(edges, rect, 0, 0, edges.Width, edges.Height, GraphicsUnit.Pixel, attrs);
                }
            }

            return result;
        }

        #endregion 边缘检测 - Sobel 算子

        #endregion 图像处理方法

        #region 高斯模糊

        /// <summary>
        /// 高斯模糊
        /// </summary>
        /// <param name="bitmap">原图</param>
        /// <param name="sigma">标准差（越大越模糊）</param>
        /// <param name="kernelSize">卷积核大小（奇数，如 5, 7, 9）</param>
        public static Bitmap GaussianBlur(Bitmap bitmap, double sigma = 2.0, int kernelSize = 5)
        {
            if (kernelSize % 2 == 0) kernelSize++; // 必须为奇数

            // 生成高斯核
            double[,] kernel = GenerateGaussianKernel(sigma, kernelSize);
            double sum = 0;
            for (int i = 0; i < kernelSize; i++)
                for (int j = 0; j < kernelSize; j++)
                    sum += kernel[i, j];

            // 归一化
            for (int i = 0; i < kernelSize; i++)
                for (int j = 0; j < kernelSize; j++)
                    kernel[i, j] /= sum;

            return Convolve(bitmap, kernel);
        }

        private static double[,] GenerateGaussianKernel(double sigma, int size)
        {
            double[,] kernel = new double[size, size];
            int center = size / 2;
            double sum = 0;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    double dx = x - center;
                    double dy = y - center;
                    kernel[y, x] = Math.Exp(-(dx * dx + dy * dy) / (2 * sigma * sigma));
                    sum += kernel[y, x];
                }
            }

            // 归一化
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    kernel[y, x] /= sum;

            return kernel;
        }

        private static Bitmap Convolve(Bitmap bitmap, double[,] kernel)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int radius = kernel.GetLength(0) / 2;
            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData resultData = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                byte* resultPtr = (byte*)resultData.Scan0;
                int stride = data.Stride;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double r = 0, g = 0, b = 0;

                        for (int ky = -radius; ky <= radius; ky++)
                        {
                            for (int kx = -radius; kx <= radius; kx++)
                            {
                                int nx = x + kx;
                                int ny = y + ky;

                                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                                {
                                    int srcIdx = (ny * stride) + (nx * 4);
                                    double weight = kernel[ky + radius, kx + radius];

                                    b += ptr[srcIdx + 0] * weight;
                                    g += ptr[srcIdx + 1] * weight;
                                    r += ptr[srcIdx + 2] * weight;
                                }
                            }
                        }

                        int dstIdx = (y * stride) + (x * 4);
                        resultPtr[dstIdx + 0] = (byte)Math.Max(0, Math.Min(255, b));
                        resultPtr[dstIdx + 1] = (byte)Math.Max(0, Math.Min(255, g));
                        resultPtr[dstIdx + 2] = (byte)Math.Max(0, Math.Min(255, r));
                        resultPtr[dstIdx + 3] = 255; // Alpha
                    }
                }
            }

            bitmap.UnlockBits(data);
            result.UnlockBits(resultData);

            return result;
        }

        #endregion 高斯模糊

        #region 脏污检测

        /// <summary>
        /// 脏污检测结果
        /// </summary>
        public class StainDetectionResult
        {
            public bool HasStains { get; set; }
            public List<Rectangle> StainRegions { get; set; } = new List<Rectangle>();
            public int TotalStainPixels { get; set; }

            public string Summary => HasStains
                ? $"检测到脏污：{StainRegions.Count} 个区域，共 {TotalStainPixels} 像素"
                : "未检测到明显脏污";
        }

        /// <summary>
        /// 脏污检测（基于局部灰度均值与方差）
        /// </summary>
        /// <param name="bitmap">输入图像</param>
        /// <param name="blockSize">分块大小（如 32x32）</param>
        /// <param name="darkThreshold">灰度低于此值认为是脏污（0-255）</param>
        /// <param name="varianceThreshold">方差高于此值表示纹理复杂（可能为污渍边缘）</param>
        public static StainDetectionResult DetectStains(Bitmap bitmap, int blockSize = 32, byte darkThreshold = 50, double varianceThreshold = 100)
        {
            var result = new StainDetectionResult();
            var stains = new List<Rectangle>();

            Bitmap gray = ToGrayscale(bitmap);
            int blocksX = (int)Math.Ceiling((double)bitmap.Width / blockSize);
            int blocksY = (int)Math.Ceiling((double)bitmap.Height / blockSize);

            int totalStainPixels = 0;

            for (int by = 0; by < blocksY; by++)
            {
                for (int bx = 0; bx < blocksX; bx++)
                {
                    int x = bx * blockSize;
                    int y = by * blockSize;
                    int w = Math.Min(blockSize, bitmap.Width - x);
                    int h = Math.Min(blockSize, bitmap.Height - y);

                    double mean = 0, variance = 0;
                    int pixelCount = 0;

                    for (int iy = 0; iy < h; iy++)
                    {
                        for (int ix = 0; ix < w; ix++)
                        {
                            Color c = gray.GetPixel(x + ix, y + iy);
                            int grayValue = c.R;
                            mean += grayValue;
                            pixelCount++;
                        }
                    }

                    if (pixelCount == 0) continue;

                    mean /= pixelCount;

                    for (int iy = 0; iy < h; iy++)
                    {
                        for (int ix = 0; ix < w; ix++)
                        {
                            Color c = gray.GetPixel(x + ix, y + iy);
                            int grayValue = c.R;
                            variance += (grayValue - mean) * (grayValue - mean);
                        }
                    }

                    variance /= pixelCount;

                    // 判断是否为脏污区域
                    if (mean < darkThreshold && variance > varianceThreshold)
                    {
                        stains.Add(new Rectangle(x, y, w, h));
                        totalStainPixels += pixelCount;
                    }
                }
            }

            result.HasStains = stains.Count > 0;
            result.StainRegions = stains;
            result.TotalStainPixels = totalStainPixels;

            return result;
        }

        #endregion 脏污检测

        ///// <summary>
        ///// 模板比对函数 图片比对函数
        ///// </summary>
        ///// <param name="imagePath1"></param>
        ///// <param name="imagePath2"></param>
        ///// <returns></returns>
        ///// <exception cref="Exception"></exception>
        //public static double CompareImageSimilarity(string imagePath1, string imagePath2)
        //{
        //    using (Mat img1 = Cv2.ImRead(imagePath1, ImreadModes.Color))
        //    using (Mat img2 = Cv2.ImRead(imagePath2, ImreadModes.Color))
        //    {
        //        if (img1.Empty() || img2.Empty())
        //        {
        //            throw new Exception("图像加载失败，请检查路径");
        //        }

        //        // 调整大小为相同尺寸（可选）
        //        Cv2.Resize(img1, img1, new OpenCvSharp.Size(500, 500));
        //        Cv2.Resize(img2, img2, new OpenCvSharp.Size(500, 500));

        //        // 转为 HSV 色彩空间（对光照变化更鲁棒）
        //        using (Mat hsv1 = new Mat())
        //        using (Mat hsv2 = new Mat())
        //        {
        //            Cv2.CvtColor(img1, hsv1, ColorConversionCodes.BGR2HSV);
        //            Cv2.CvtColor(img2, hsv2, ColorConversionCodes.BGR2HSV);

        //            // 计算直方图
        //            int[] channels = { 0, 1 }; // H 和 S 通道
        //            int[] histSize = { 50, 60 }; // H:50 bins, S:60 bins
        //            Rangef[] ranges = { new Rangef(0, 180), new Rangef(0, 256) };

        //            using (Mat hist1 = new Mat())
        //            using (Mat hist2 = new Mat())
        //            {
        //                Cv2.CalcHist(new[] { hsv1 }, channels, null, hist1, 2, histSize, ranges);
        //                Cv2.CalcHist(new[] { hsv2 }, channels, null, hist2, 2, histSize, ranges);

        //                // 归一化
        //                Cv2.Normalize(hist1, hist1, 0, 1, NormTypes.MinMax);
        //                Cv2.Normalize(hist2, hist2, 0, 1, NormTypes.MinMax);

        //                // 比较直方图（方法 0: Correlation，值越接近 1 越相似）
        //                double similarity = Cv2.CompareHist(hist1, hist2, HistCompMethods.Correl);

        //                return similarity; // 返回 0.0 ~ 1.0，1 表示完全相同
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// 计算两张图片的相似度（基于模板匹配）
        /// </summary>
        /// <param name="sourcePath">源图像路径</param>
        /// <param name="templatePath">模板图像路径</param>
        /// <param name="method">匹配方法，默认使用归一化相关系数</param>
        /// <returns>相似度分数（0~1），越接近1表示越相似</returns>
        public static double CompareImageSimilarity(string sourcePath, string templatePath, TemplateMatchModes method = TemplateMatchModes.CCoeffNormed)
        {
            using (var src = Cv2.ImRead(sourcePath, ImreadModes.Grayscale))
            using (var tpl = Cv2.ImRead(templatePath, ImreadModes.Grayscale))
            {
                if (src.Empty() || tpl.Empty())
                    throw new ArgumentException("无法读取图像文件，请检查路径。");

                if (tpl.Rows > src.Rows || tpl.Cols > src.Cols)
                    throw new ArgumentException("模板图像不能大于源图像。");

                // 执行模板匹配
                using (var result = new Mat())
                {
                    Cv2.MatchTemplate(src, tpl, result, method);

                    // 归一化结果（可选，但推荐）
                    Cv2.Normalize(result, result, 0, 1, NormTypes.MinMax);

                    // 获取最佳匹配位置的值（即最大相似度）
                    Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out _);

                    // 对于 CCoeffNormed 和 SQDIFF_NORMED，maxVal 即为相似度
                    // 注意：SQDIFF_NORMED 越小越相似，但这里我们统一返回 0~1，越大越相似
                    if (method == TemplateMatchModes.SqDiffNormed)
                    {
                        maxVal = 1 - maxVal; // 反转，使其符合“越大越相似”
                    }

                    return Math.Max(0, Math.Min(1, maxVal)); // 限制在 0~1 范围
                }
            }
        }

        /// <summary>
        /// 直接从内存中的 Mat 对象进行比对
        /// </summary>
        public static double CompareImageSimilarity(Mat source, Mat template, TemplateMatchModes method = TemplateMatchModes.CCoeffNormed)
        {
            if (source.Empty() || template.Empty())
                throw new ArgumentException("图像为空。");

            if (template.Rows > source.Rows || template.Cols > source.Cols)
                throw new ArgumentException("模板图像不能大于源图像。");

            using (var result = new Mat())
            {
                Cv2.MatchTemplate(source, template, result, method);
                Cv2.Normalize(result, result, 0, 1, NormTypes.MinMax);
                Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out _);

                if (method == TemplateMatchModes.SqDiffNormed)
                    maxVal = 1 - maxVal;

                return Math.Max(0, Math.Min(1, maxVal));
            }
        }

        /// <summary>
        /// 使用 SIFT 特征点匹配计算图像相似度（适配新版 OpenCvSharp）
        /// </summary>
        public static double CompareWithSIFT(string sourcePath, string templatePath)
        {
            using (var src = Cv2.ImRead(sourcePath, ImreadModes.Grayscale))
            using (var tpl = Cv2.ImRead(templatePath, ImreadModes.Grayscale))
            {
                if (src.Empty() || tpl.Empty())
                    throw new ArgumentException("无法加载图像");

                var sift = SIFT.Create(1000); // 创建 SIFT 检测器

                // 提取特征点和描述子
                KeyPoint[] kp1;
                Mat des1 = new Mat();
                sift.DetectAndCompute(src, null, out kp1, des1);

                KeyPoint[] kp2;
                Mat des2 = new Mat();
                sift.DetectAndCompute(tpl, null, out kp2, des2);

                if (des1.Rows == 0 || des2.Rows == 0)
                {
                    des1.Dispose();
                    des2.Dispose();
                    return 0;
                }

                try
                {
                    using (var matcher = new FlannBasedMatcher())
                    {
                        var matches = matcher.KnnMatch(des1, des2, k: 2);

                        var goodMatches = matches
                            .Where(m => m.Length == 2 && m[0].Distance < 0.7 * m[1].Distance)
                            .Select(m => m[0])
                            .ToList();

                        double similarity = 0;

                        if (goodMatches.Count >= 4)
                        {
                            var srcPts = goodMatches.Select(m => kp1[m.QueryIdx].Pt).ToArray();
                            var dstPts = goodMatches.Select(m => kp2[m.TrainIdx].Pt).ToArray();

                            using (var srcInput = InputArray.Create(srcPts))
                            using (var dstInput = InputArray.Create(dstPts))
                            using (var mask = new Mat())
                            {
                                Cv2.FindHomography(srcInput, dstInput, HomographyMethods.Ransac, 3, mask);
                                int inliers = Cv2.CountNonZero(mask);
                                similarity = (double)inliers / Math.Max(kp1.Length, kp2.Length);
                            }
                        }
                        else
                        {
                            similarity = (double)goodMatches.Count / Math.Max(kp1.Length, kp2.Length);
                        }

                        return Math.Min(1.0, similarity);
                    }
                }
                finally
                {
                    des1.Dispose();
                    des2.Dispose();
                }
            }
        }
    }
}