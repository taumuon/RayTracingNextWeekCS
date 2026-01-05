using StbImageSharp;
using System.Numerics;

namespace RayTracerNextWeekCS
{
    public class ImageTexture : ITexture
    {
        private int imageWidth;
        private int imageHeight;
        private byte[] data;

        public ImageTexture(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlue);
                //byte[] buffer = File.ReadAllBytes(path);
                //ImageResult image = ImageResult.FromMemory(buffer, ColorComponents.RedGreenBlueAlpha);
                imageWidth = image.Width;
                imageHeight = image.Height;
                data = image.Data;
            }
        }

        public Vector3 ColorValue(double u, double v, Vector3 p)
        {
            // Clamp input texture coordinates to [0,1] x [1,0]
            u = new Interval(0, 1).Clamp(u);
            v = 1.0 - new Interval(0, 1).Clamp(v);  // Flip V to image coordinates

            var i = (int)(u * imageWidth);
            var j = (int)(v * imageHeight);
            if (i == imageWidth)
            {
                i = imageWidth - 1;
            }
            if (j == imageHeight)
            {
                j = imageHeight - 1;
            }

            var pointIndex = (int)(3 * (j * imageWidth + i));
            byte r = data[pointIndex];
            byte g = data[pointIndex + 1];
            byte b = data[pointIndex + 2];

            var colorScale = 1.0 / 255.0;
            return new Vector3((float)(colorScale * r), (float)(colorScale * g), (float)(colorScale * b));
        }
    }
}
