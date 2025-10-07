using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;

namespace NetAIVision.Model
{
    public class QRCodeHelper
    {
        /// <summary>
        /// 从图像中识别二维码，并返回二维码文本与坐标信息
        /// </summary>
        public static (string text, List<Point> points) ReturnBarcodeCoordinates(Bitmap bitmap)
        {
            var reader = new BarcodeReader
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

            var result = reader.Decode(bitmap);  // ✅ 这里可直接传入 Bitmap

            if (result == null)
                return (null, null);

            var points = new List<Point>();
            if (result.ResultPoints != null && result.ResultPoints.Length > 0)
            {
                foreach (var p in result.ResultPoints)
                {
                    points.Add(new Point((int)p.X, (int)p.Y));
                }
            }

            return (result.Text, points);
        }

        /// <summary>
        /// 绘制二维码识别框
        /// </summary>
        public static Bitmap DrawQRCodeBox(Bitmap bitmap, List<Point> points)
        {
            if (points == null || points.Count == 0)
                return bitmap;

            using (var g = Graphics.FromImage(bitmap))
            {
                using (var pen = new Pen(Color.Lime, 3))
                {
                    // 连成闭环
                    for (int i = 0; i < points.Count; i++)
                    {
                        var p1 = points[i];
                        var p2 = points[(i + 1) % points.Count];
                        g.DrawLine(pen, p1, p2);
                    }
                }
            }

            return bitmap;
        }
    }
}