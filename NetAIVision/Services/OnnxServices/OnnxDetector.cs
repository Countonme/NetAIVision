using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace NetAIVision.Services.OnnxServices
{
    public class OnnxDetector : IDisposable
    {
        private InferenceSession _session;
        private string _inputName;
        private int _inputHeight;
        private int _inputWidth;
        private float _confidenceThreshold;

        public class DetectionResult
        {
            public Rectangle Box;       // System.Drawing.Rectangle（便于 WinForm/UI 使用）
            public float Confidence;
            public int ClassId;
        }

        private class YoloBox
        {
            public Rect Box;     // OpenCvSharp.Rect
            public float Conf;
            public int Label;
        }

        private static float Sigmoid(float x)
        {
            return 1f / (1f + (float)Math.Exp(-x));
        }

        public OnnxDetector(string modelPath, float confidenceThreshold = 0.4f, bool useGpu = false)
        {
            var options = new SessionOptions();
            if (useGpu)
            {
                try
                {
                    options.AppendExecutionProvider_CUDA(0); // 指定 GPU 0
                }
                catch
                {
                    System.Console.WriteLine("⚠️ GPU 不可用，回退到 CPU 模式");
                }
            }

            _session = new InferenceSession(modelPath, options);
            _inputName = _session.InputMetadata.Keys.First();
            var dims = _session.InputMetadata[_inputName].Dimensions;
            _inputHeight = dims[2];  // 通常为 640
            _inputWidth = dims[3];   // 通常为 640
            _confidenceThreshold = confidenceThreshold;
        }

        /// <summary>
        /// 对 Bitmap 进行目标检测，返回检测结果列表
        /// </summary>
        public List<DetectionResult> Detect(Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));

            Mat img = BitmapToMat(bitmap);
            if (img.Empty()) throw new ArgumentException("Bitmap 转换失败");

            try
            {
                // 1. 预处理
                float[] blob;
                float ratio;
                OpenCvSharp.Point pad;
                Preprocess(img, _inputWidth, out blob, out ratio, out pad);

                // 2. 推理
                var inputTensor = new DenseTensor<float>(blob, new int[] { 1, 3, _inputHeight, _inputWidth });
                var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(_inputName, inputTensor) };

                using (var results = _session.Run(inputs))
                {
                    var output = results.First().AsTensor<float>();
                    int numClasses = output.Dimensions[1] - 4; // 自动推断类别数（84 - 4 = 80）
                    int numPreds = output.Dimensions[2];       // 如 8400

                    List<YoloBox> rawBoxes = new List<YoloBox>();

                    for (int i = 0; i < numPreds; i++)
                    {
                        float maxCls = -1e9f;
                        int clsId = 0;

                        for (int c = 4; c < 4 + numClasses; c++)
                        {
                            float clsScore = output[0, c, i];
                            if (clsScore > maxCls)
                            {
                                maxCls = clsScore;
                                clsId = c - 4;
                            }
                        }

                        float objConf = Sigmoid(output[0, 4, i]);
                        float clsConf = Sigmoid(maxCls);
                        float conf = objConf * clsConf;

                        if (conf < _confidenceThreshold) continue;

                        float cx = output[0, 0, i];
                        float cy = output[0, 1, i];
                        float w = output[0, 2, i];
                        float h = output[0, 3, i];

                        float x1 = cx - w * 0.5f;
                        float y1 = cy - h * 0.5f;
                        float x2 = cx + w * 0.5f;
                        float y2 = cy + h * 0.5f;

                        // 反 Letterbox
                        x1 = (x1 - pad.X) / ratio;
                        y1 = (y1 - pad.Y) / ratio;
                        x2 = (x2 - pad.X) / ratio;
                        y2 = (y2 - pad.Y) / ratio;

                        Rect rect = new Rect(
                            (int)Math.Max(0, x1),
                            (int)Math.Max(0, y1),
                            (int)Math.Max(0, x2 - x1),
                            (int)Math.Max(0, y2 - y1)
                        );

                        rawBoxes.Add(new YoloBox { Box = rect, Conf = conf, Label = clsId });
                    }

                    // 3. NMS
                    var nmsBoxes = NMS(rawBoxes, 0.45f);

                    // 4. 转为 System.Drawing.Rectangle 便于 UI 使用
                    List<DetectionResult> resultsList = new List<DetectionResult>();
                    foreach (var b in nmsBoxes)
                    {
                        resultsList.Add(new DetectionResult
                        {
                            Box = new Rectangle(b.Box.X, b.Box.Y, b.Box.Width, b.Box.Height),
                            Confidence = b.Conf,
                            ClassId = b.Label
                        });
                    }

                    return resultsList;
                }
            }
            finally
            {
                img.Dispose();
            }
        }

        /// <summary>
        /// 在原图上绘制检测框并返回新 Bitmap
        /// </summary>
        public Bitmap DrawDetections(Bitmap original, List<DetectionResult> detections)
        {
            Mat mat = BitmapToMat(original);
            try
            {
                foreach (var det in detections)
                {
                    Scalar color = Scalar.RandomColor();
                    Cv2.Rectangle(mat, new Rect(det.Box.X, det.Box.Y, det.Box.Width, det.Box.Height), color, 2);
                    string label = $"ID:{det.ClassId} {det.Confidence:F2}";
                    Cv2.PutText(mat, label, new OpenCvSharp.Point(det.Box.X, Math.Max(10, det.Box.Y - 5)),
                        HersheyFonts.HersheySimplex, 0.5, color, 1);
                }
                return MatToBitmap(mat);
            }
            finally
            {
                mat.Dispose();
            }
        }

        #region Helpers

        private static void Preprocess(Mat img, int size, out float[] tensor, out float ratio, out OpenCvSharp.Point pad)
        {
            int w = img.Width;
            int h = img.Height;
            ratio = Math.Min(size / (float)w, size / (float)h);
            int newW = (int)(w * ratio);
            int newH = (int)(h * ratio);
            int dw = (size - newW) / 2;
            int dh = (size - newH) / 2;

            pad = new OpenCvSharp.Point(dw, dh);

            Mat resized = new Mat();
            Cv2.Resize(img, resized, new OpenCvSharp.Size(newW, newH));

            Mat padded = new Mat();
            Cv2.CopyMakeBorder(resized, padded, dh, size - newH - dh, dw, size - newW - dw,
                BorderTypes.Constant, new Scalar(114, 114, 114));

            Cv2.CvtColor(padded, padded, ColorConversionCodes.BGR2RGB);
            padded.ConvertTo(padded, MatType.CV_32F, 1.0f / 255.0f);

            tensor = new float[3 * size * size];
            int idx = 0;
            for (int c = 0; c < 3; c++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        Vec3f pixel = padded.At<Vec3f>(y, x);
                        tensor[idx++] = pixel[c];
                    }
                }
            }
        }

        private static List<YoloBox> NMS(List<YoloBox> boxes, float iouThreshold)
        {
            var sorted = boxes.OrderByDescending(b => b.Conf).ToList();
            List<YoloBox> keep = new List<YoloBox>();

            while (sorted.Count > 0)
            {
                var pick = sorted[0];
                keep.Add(pick);
                sorted.RemoveAt(0);

                for (int i = sorted.Count - 1; i >= 0; i--)
                {
                    if (IoU(pick.Box, sorted[i].Box) >= iouThreshold)
                    {
                        sorted.RemoveAt(i);
                    }
                }
            }
            return keep;
        }

        private static float IoU(Rect a, Rect b)
        {
            int x1 = Math.Max(a.X, b.X);
            int y1 = Math.Max(a.Y, b.Y);
            int x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            int y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            int interArea = Math.Max(0, x2 - x1) * Math.Max(0, y2 - y1);
            int unionArea = a.Width * a.Height + b.Width * b.Height - interArea;

            return unionArea == 0 ? 0f : (float)interArea / unionArea;
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

        #endregion Helpers

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}