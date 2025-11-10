using OpenCvSharp;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace NetAIVision.Services
{
    public static class TextOrientationCorrector
    {
        public static Bitmap RemoveBorderSymbols(Bitmap bitmap)
        {
            // 轉為 Mat
            Mat src = ImageAlignment.BitmapToMat(bitmap);
            Mat gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            // 偵測邊緣
            Mat edges = new Mat();
            Cv2.Canny(gray, edges, 80, 200);

            // 擴大邊緣區域 (讓符號都包含進去)
            Cv2.Dilate(edges, edges, Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3)));

            // 計算背景顏色（取整張圖的平均值）
            Scalar bgColor = Cv2.Mean(src, ~edges);

            // 把邊緣區域填成背景色
            src.SetTo(bgColor, edges);

            return ImageAlignment.MatToBitmap(src);
        }

        /// <summary>
        /// 根據文字傾斜自動糾偏
        /// </summary>
        public static Bitmap Deskew(Bitmap bitmap)
        {
            var src = ImageAlignment.BitmapToMat(bitmap);
            var gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            // 二值化
            Cv2.Threshold(gray, gray, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

            // 取反，使文字為白色背景為黑色
            Cv2.BitwiseNot(gray, gray);

            // 尋找非零點（文字區域）
            var nonZero = new Mat();
            Cv2.FindNonZero(gray, nonZero);
            if (nonZero.Empty())
                return bitmap; // 沒有文字就不處理

            // 計算最小外接矩形
            var box = Cv2.MinAreaRect(nonZero);
            double angle = box.Angle;

            // 校正角度：OpenCV 的角度是以垂直方向為基準
            if (angle < -45)
                angle += 90;

            // 旋轉圖片
            var center = new Point2f(src.Width / 2f, src.Height / 2f);
            var rotMat = Cv2.GetRotationMatrix2D(center, angle, 1.0);
            var rotated = new Mat();
            Cv2.WarpAffine(src, rotated, rotMat, new OpenCvSharp.Size(src.Width, src.Height),
                           InterpolationFlags.Linear, BorderTypes.Reflect101);

            return ImageAlignment.MatToBitmap(rotated);
        }

        public static Bitmap CorrectTextOrientation(Bitmap srcBitmap)
        {
            if (srcBitmap == null) throw new ArgumentNullException(nameof(srcBitmap));

            using (Mat src = ImageAlignment.BitmapToMat(srcBitmap))
            using (Mat gray = new Mat())
            using (Mat edges = new Mat())
            {
                // 1️⃣ 灰階
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

                // 2️⃣ 邊緣檢測
                Cv2.Canny(gray, edges, 50, 150);

                // 3️⃣ 霍夫直線檢測
                LineSegmentPoint[] lines = Cv2.HoughLinesP(
                    edges,
                    1, Math.PI / 180, // 精度
                    50,                // 最小直線長度閾值
                    30,                // 最小連接間距
                    10                 // 阈值
                );

                if (lines.Length == 0)
                    return (Bitmap)srcBitmap.Clone();

                // 4️⃣ 計算所有直線的平均角度
                double sumAngle = 0;
                int count = 0;
                foreach (var line in lines)
                {
                    double dx = line.P2.X - line.P1.X;
                    double dy = line.P2.Y - line.P1.Y;
                    if (Math.Abs(dx) < 1e-3) continue; // 避免垂直線
                    double angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
                    if (Math.Abs(angle) < 45) // 只取接近水平的線
                    {
                        sumAngle += angle;
                        count++;
                    }
                }

                if (count == 0)
                    return (Bitmap)srcBitmap.Clone();

                double avgAngle = sumAngle / count;
                Console.WriteLine($"Detected text tilt angle: {avgAngle:F2}°");

                // 5️⃣ 旋轉修正
                Point2f center = new Point2f(src.Width / 2f, src.Height / 2f);
                Mat rotMat = Cv2.GetRotationMatrix2D(center, avgAngle, 1.0);
                Mat rotated = new Mat();
                Cv2.WarpAffine(src, rotated, rotMat, new OpenCvSharp.Size(src.Width, src.Height),
                               InterpolationFlags.Linear, BorderTypes.Reflect);

                return ImageAlignment.MatToBitmap(rotated);
            }
        }
    }
}