using NetAIVision.Model;
using OpenCvSharp;
using Sunny.UI;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ZXing;

namespace NetAIVision.Controller
{
    public partial class FrmQRCode : UIForm
    {
        public FrmQRCode()
        {
            InitializeComponent();
            //pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            //pictureBox1.BorderStyle = BorderStyle.FixedSingle;
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(dlg.FileName);
                    pictureBox1.Image = bmp;

                    var (text, points) = QRCodeHelper.ReturnBarcodeCoordinates(bmp);

                    if (!string.IsNullOrEmpty(text))
                    {
                        // 绘制二维码框
                        var imgWithBox = QRCodeHelper.DrawQRCodeBox(new Bitmap(bmp), points);
                        pictureBox1.Image = imgWithBox;
                        MessageBox.Show($"识别成功：{text}");
                    }
                    else
                    {
                        MessageBox.Show("未识别到二维码");
                    }
                }
            }
        }

        //private string RecognizeQRCode_LaserOptimized(Bitmap bmp)
        //{
        //    // Bitmap → Mat
        //    Mat src;
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        //        ms.Position = 0;
        //        src = Mat.FromStream(ms, ImreadModes.Color);
        //    }

        //    // 转灰度
        //    Mat gray = new Mat();
        //    Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

        //    // ==== Step 1: 图像增强 ====
        //    // 1. 降噪 + 对比度增强
        //    Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(3, 3), 0);
        //    Cv2.EqualizeHist(gray, gray);

        //    // 2. 自适应阈值（处理镭射浅色二维码）
        //    Mat binary = new Mat();
        //    Cv2.AdaptiveThreshold(gray, binary, 255,
        //        AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 35, 5);

        //    // 3. 同时准备反色版本
        //    Mat inverted = new Mat();
        //    Cv2.BitwiseNot(binary, inverted);

        //    // 4. 放大 2~3 倍，提高 ZXing 的采样分辨率
        //    //Cv2.Resize(binary, binary, new OpenCvSharp.Size(binary.Width * 3, binary.Height * 3));
        //    // Cv2.Resize(inverted, inverted, new OpenCvSharp.Size(inverted.Width * 3, inverted.Height * 3));

        //    // ==== Step 2: 先试 ZXing（支持 DataMatrix） ====
        //    var reader = new ZXing.BarcodeReader
        //    {
        //        AutoRotate = true,
        //        TryInverted = true,
        //        Options = new ZXing.Common.DecodingOptions
        //        {
        //            TryHarder = true,
        //            PossibleFormats = new List<ZXing.BarcodeFormat> {
        //                ZXing.BarcodeFormat.QR_CODE,
        //                ZXing.BarcodeFormat.DATA_MATRIX,
        //                ZXing.BarcodeFormat.AZTEC,
        //                ZXing.BarcodeFormat.PDF_417
        //            },
        //        }
        //    };

        //    // ZXing 通常不接受 Mat，转 Bitmap
        //    Bitmap bmpBinary, bmpInverted;
        //    using (MemoryStream ms1 = new MemoryStream())
        //    {
        //        Cv2.ImEncode(".bmp", binary, out var buf);
        //        ms1.Write(buf, 0, buf.Length);
        //        ms1.Position = 0;
        //        bmpBinary = new Bitmap(ms1);
        //    }
        //    using (MemoryStream ms2 = new MemoryStream())
        //    {
        //        Cv2.ImEncode(".bmp", inverted, out var buf2);
        //        ms2.Write(buf2, 0, buf2.Length);
        //        ms2.Position = 0;
        //        bmpInverted = new Bitmap(ms2);
        //    }

        //    var result = reader.Decode(bmpBinary) ?? reader.Decode(bmpInverted);
        //    if (result != null)
        //    {
        //        MessageBox.Show($"识别成功：{result.BarcodeFormat} → {result.Text}");
        //        return result.Text;
        //    }

        //    // ==== Step 3: 备用 OpenCV QRCodeDetector ====
        //    //var detector = new QRCodeDetector();
        //    // string text = detector.DetectAndDecode(binary, out Point2f[] points);
        //    // if (string.IsNullOrEmpty(text))
        //    //    text = detector.DetectAndDecode(inverted, out points);

        //    if (!string.IsNullOrEmpty(text))
        //    {
        //        DrawBox(src, points);
        //        UpdatePictureBox(src);
        //        MessageBox.Show($"（OpenCV）识别成功：{text}");
        //        return text;
        //    }

        //    MessageBox.Show("未识别到二维码或数据矩阵码。");
        //    return null;
        //}

        private void DrawBox(Mat image, Point2f[] points)
        {
            if (points != null && points.Length == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    Cv2.Line(image,
                        (OpenCvSharp.Point)points[i],
                        (OpenCvSharp.Point)points[(i + 1) % 4],
                        Scalar.Red, 3);
                }
            }
        }

        private void UpdatePictureBox(Mat mat)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Cv2.ImEncode(".bmp", mat, out var data);
                ms.Write(data, 0, data.Length);
                ms.Position = 0;
                pictureBox1.Image = new Bitmap(ms);
                pictureBox1.Refresh();
            }
        }
    }
}