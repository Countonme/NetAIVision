using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAIVision.Services
{
    public class BitmapProcessorServices
    {
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
                g.DrawImage(bitmap, new Point(0, 0));
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
                g.DrawImage(bitmap, new Point(0, 0));
            }
            return result;
        }

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
    }
}