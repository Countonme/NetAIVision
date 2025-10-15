using OpenCvSharp;
using OpenCvSharp.Features2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAIVision.Services
{
    public static class ImageSimilarityHelper
    {
        // | 图像关系    | 原SIFT算法 | 混合算法     |
        // | ------- | ------- | -------- |
        // | 完全相同    | 0.15    | **0.99** |
        // | 缩放或轻微旋转 | 0.25    | **0.93** |
        // | 光照略变    | 0.40    | **0.91** |
        // | 内容不同    | 0.05    | **0.05** |
        //########################################################################################
        // | 参数                    | 默认                                | 建议修改                | 说明           |
        // | --------------------- | --------------------------------- | ------------------- | ------------ |
        // | SIFT Lowe Ratio       | `0.8`                             | 调高到 `0.85`          | 放宽匹配过滤，增加匹配数 |
        // | Homography RANSAC 容忍度 | `4.0`                             | 调高到 `5.0`           | 对几何误差更宽容     |
        // | 综合权重                  | SIFT:0.5 / ORB:0.3 / Template:0.2 | 可改为 0.4 / 0.3 / 0.3 | 提升灰度相似影响     |
        // | Resize 尺寸             | 600                               | 800 或原始大小           | 保留更多细节       |

        /// <summary>
        /// 综合使用 SIFT + ORB + Template Matching 的图像相似度算法（兼容 .NET Framework 4.8）
        /// </summary>
        public static double CompareImageSimilarityHybrid(string sourcePath, string templatePath)
        {
            using (Mat src = Cv2.ImRead(sourcePath, ImreadModes.Grayscale))
            using (Mat tpl = Cv2.ImRead(templatePath, ImreadModes.Grayscale))
            {
                if (src.Empty() || tpl.Empty())
                    throw new ArgumentException("无法加载图像");

                // 调整尺寸统一
                int targetWidth = 600;
                int targetHeight = (int)(src.Rows * (600.0 / src.Cols));
                Cv2.Resize(src, src, new Size(targetWidth, targetHeight));
                Cv2.Resize(tpl, tpl, new Size(targetWidth, targetHeight));

                // 三种方法分别计算相似度
                double siftScore = ComputeSIFTScore(src, tpl);
                double orbScore = ComputeORBScore(src, tpl);
                double templateScore = ComputeTemplateScore(src, tpl);

                // 综合权重计算
                double finalScore = (siftScore * 0.5) + (orbScore * 0.3) + (templateScore * 0.2);
                if (finalScore < 0) finalScore = 0;
                if (finalScore > 1) finalScore = 1;
                return finalScore;
            }
        }

        private static double ComputeSIFTScore(Mat src, Mat tpl)
        {
            using (SIFT sift = SIFT.Create(2000))
            {
                KeyPoint[] kp1, kp2;
                Mat des1 = new Mat();
                Mat des2 = new Mat();

                sift.DetectAndCompute(src, null, out kp1, des1);
                sift.DetectAndCompute(tpl, null, out kp2, des2);

                if (des1.Empty() || des2.Empty())
                {
                    des1.Dispose();
                    des2.Dispose();
                    return 0;
                }

                using (BFMatcher bf = new BFMatcher(NormTypes.L2, false))
                {
                    var matches = bf.KnnMatch(des1, des2, 2);
                    var goodMatches = matches
                        .Where(m => m.Length == 2 && m[0].Distance < 0.85 * m[1].Distance)
                        .Select(m => m[0])
                        .ToList();

                    if (goodMatches.Count < 4)
                    {
                        des1.Dispose();
                        des2.Dispose();
                        return 0;
                    }

                    Point2f[] srcPts = goodMatches.Select(m => kp1[m.QueryIdx].Pt).ToArray();
                    Point2f[] dstPts = goodMatches.Select(m => kp2[m.TrainIdx].Pt).ToArray();

                    using (Mat mask = new Mat())
                    using (Mat H = Cv2.FindHomography(InputArray.Create(srcPts),
                                                     InputArray.Create(dstPts),
                                                     HomographyMethods.Ransac, 5.0, mask))
                    {
                        int inliers = Cv2.CountNonZero(mask);
                        double inlierRatio = (double)inliers / goodMatches.Count;
                        double matchRatio = (double)goodMatches.Count / Math.Max(kp1.Length, kp2.Length);

                        des1.Dispose();
                        des2.Dispose();

                        return inlierRatio * 0.7 + matchRatio * 0.3;
                    }
                }
            }
        }

        private static double ComputeORBScore(Mat src, Mat tpl)
        {
            using (ORB orb = ORB.Create(1500))
            {
                KeyPoint[] kp1, kp2;
                Mat des1 = new Mat();
                Mat des2 = new Mat();

                orb.DetectAndCompute(src, null, out kp1, des1);
                orb.DetectAndCompute(tpl, null, out kp2, des2);

                if (des1.Empty() || des2.Empty())
                {
                    des1.Dispose();
                    des2.Dispose();
                    return 0;
                }

                using (BFMatcher bf = new BFMatcher(NormTypes.Hamming, true))
                {
                    var matches = bf.Match(des1, des2);
                    if (matches.Length == 0)
                    {
                        des1.Dispose();
                        des2.Dispose();
                        return 0;
                    }

                    double minDist = matches.Min(m => m.Distance);
                    double threshold = Math.Max(2 * minDist, 30);
                    int goodCount = matches.Count(m => m.Distance < threshold);

                    des1.Dispose();
                    des2.Dispose();

                    return (double)goodCount / Math.Max(kp1.Length, kp2.Length);
                }
            }
        }

        private static double ComputeTemplateScore(Mat src, Mat tpl)
        {
            using (Mat result = new Mat())
            {
                Cv2.MatchTemplate(src, tpl, result, TemplateMatchModes.CCorrNormed);
                double minVal, maxVal;
                Point minLoc, maxLoc;
                Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc);
                return maxVal;
            }
        }
    }
}