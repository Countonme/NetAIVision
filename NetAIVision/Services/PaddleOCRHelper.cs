using OpenCvSharp;

using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models.Local;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace NetAIVision.Services
{
    /// <summary>
    /// PaddleOCR 圖像識別封裝類，支持直接傳 Bitmap
    /// </summary>
    public static class PaddleOCRHelper
    {
        private static PaddleOcrAll _ocr;
        private static readonly object _lock = new object();

        /// <summary>
        /// 初始化 OCR 模型，只在首次使用時執行
        /// </summary>
        private static void EnsureInitialized()
        {
            if (_ocr != null) return;

            lock (_lock)
            {
                if (_ocr != null) return;

                Console.WriteLine("🔍 初始化 PaddleOCR 模型...");

                // var model = LocalFullModels.ChineseV3;   // 中文+英文模型
                var model = LocalFullModels.EnglishV3;   // 中文+英文模型
                var device = PaddleDevice.Mkldnn();      // CPU 加速

                _ocr = new PaddleOcrAll(model, device)
                {
                    AllowRotateDetection = true,
                    Enable180Classification = true
                };

                Console.WriteLine("✅ PaddleOCR 初始化完成");
            }
        }

        /// <summary>
        /// 從 Bitmap 圖像識別文字
        /// </summary>
        public static string Recognize(Bitmap bitmap)
        {
            EnsureInitialized();

            if (bitmap == null)
                return string.Empty;

            try
            {
                // bitmap = PreprocessForPaddleOCR(bitmap);
                // Bitmap → Mat
                Mat mat = BitmapToMat(bitmap);

                // 執行 OCR
                var result = _ocr.Run(mat);

                if (result == null)
                    return string.Empty;

                // 合併所有識別出的文字
                return string.Join(" ", result.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ OCR 失敗: {ex.Message}");
                return string.Empty;
            }
        }

        public static Bitmap PreprocessForPaddleOCR(Bitmap src)
        {
            using (var mat = BitmapToMat(src))
            {
                Mat gray = new Mat();
                Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);

                // ✅ 1. 自適應直方圖均衡 (提升對比)
                Cv2.EqualizeHist(gray, gray);

                // ✅ 2. 自適應閾值二值化 (去除背景)
                Mat binary = new Mat();
                Cv2.AdaptiveThreshold(gray, binary, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 25, 15);

                // ✅ 3. 中值濾波 (去噪)
                Cv2.MedianBlur(binary, binary, 3);

                // ✅ 4. (可選) 透視校正
                // 若你的影像是斜拍的，可以透過角點偵測 + warpPerspective 校正
                // e.g., 使用輪廓 + 四邊形近似

                return MatToBitmap(binary);
            }
        }

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
    }
}