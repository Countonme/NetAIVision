using OpenCvSharp;
using OpenCvSharp.Internal.Vectors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAIVision.Services
{
    public class ImageCorrection
    {
        public static Mat CorrectImagePerspective(Mat inputImage, Point2f[] sourcePoints, Point2f[] destinationPoints)
        {
            if (sourcePoints.Length != 4 || destinationPoints.Length != 4)
            {
                throw new ArgumentException("Source and destination points must contain exactly 4 points each.");
            }
            // Calculate the perspective transformation matrix
            Mat perspectiveMatrix = Cv2.GetPerspectiveTransform(sourcePoints, destinationPoints);
            // Apply the perspective transformation to the input image
            Mat correctedImage = new Mat();
            Cv2.WarpPerspective(inputImage, correctedImage, perspectiveMatrix, inputImage.Size());
            return correctedImage;
        }
    }
}