using System.Numerics;
using System.Text;

namespace RayTracerNextWeekCS
{
    public class Camera
    {

        // TODO: pass into c'tor instead of public
        public double AspectRatio { get; set; } = 1.0; // Ratio of image width over height

        public int ImageWidth { get; set; } = 100; // Rendered image width in pixel count

        public int SamplesPerPixel { get; set; } = 4; // Count of random samples for each pixel

        public int MaxDepth { get; set; } = 4;   // Maximum number of ray bounces into scene

        public double VFOV { get; set; } = 90; // Vertical view angle (degrees) field of view

        public Vector3 LookFrom { get; set; } = new Vector3(); // Point camera is looking from

        public Vector3 LookAt { get; set; } = new Vector3(0.0f, 0.0f, -1.0f); // point camera is looking at

        public Vector3 VUp { get; set; } = new Vector3(0.0f, 1.0f, 0.0f); // camera relative up direction

        public double DefocusAngle { get; set; } = 0.0; // Variation angle of rays through each pixel
        public double FocusDist { get; set; } = 10.0; // Distance from camera lookfrom point to plane of perfect focus

        public Vector3 Background { get; set; } // Scene background color.

        private int imageHeight;
        private double pixelSamplesScale;
        private Vector3 center;
        private Vector3 pixel00Loc;
        private Vector3 pixelDeltaU;
        private Vector3 pixelDeltaV;
        private Vector3 u, v, w; // Camera frame basis vectors
        private Vector3 defocusDiscU; // Defocus disc horizontal radius
        private Vector3 defocusDiscV; // Defocus disc vertical radius

        public Stats Render(Hittables world, string fileName)
        {
            Initialize();

            Vector3[][] image = new Vector3[imageHeight][];

            var outContent = new StringBuilder();

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var before = GC.GetAllocatedBytesForCurrentThread();


            Parallel.ForEach(Enumerable.Range(0, imageHeight), rowIndex =>
            {
                var row = new Vector3[ImageWidth];
                    for (int i = 0; i < ImageWidth; i++)
                    {
                        var pixelColor = new Vector3();
                        for (int sample = 0; sample < SamplesPerPixel; sample++)
                        {
                            var ray = GetRay(i, rowIndex);
                            var rayColor = RayColor(ray, MaxDepth, world);

                            pixelColor += rayColor;
                        }

                        row[i] = pixelColor;
                    }
                    image[rowIndex] = row;
            });

            watch.Stop();
            var after = GC.GetAllocatedBytesForCurrentThread();
            var allocated = after - before;
            var elapsed = watch.ElapsedMilliseconds;

            var output = fileName;

            outContent.AppendLine("P3");
            outContent.AppendLine($"{ImageWidth} {imageHeight}");
            outContent.AppendLine("255");

            for (int j = 0; j < imageHeight; j++)
            {
                var row = image[j];
                for (int i = 0; i < ImageWidth; i++)
                {
                    var pixelColor = row[i];
                    outContent.AppendLine(WriteColor(pixelColor * (float)pixelSamplesScale));
                }
            }

            File.WriteAllText(output, outContent.ToString());

            return new Stats(allocated, elapsed);
        }

        private Ray GetRay(int i, int j)
        {
            // Construct a camera ray originating from the defocus disc and directed at randomly sampled
            // point around the pixel location i, j.

            var offset = SampleSquare();
            var pixelSample = pixel00Loc
                + ((i + offset.X) * pixelDeltaU)
                + ((j + offset.Y) * pixelDeltaV);

            var rayOrigin = (DefocusAngle <= 0) ? center : DefocusDiscSample();
            var rayDirection = pixelSample - rayOrigin;

            var rayTime = RandomDouble();

            return new Ray(rayOrigin, rayDirection, rayTime);
        }

        private Vector3 DefocusDiscSample()
        {
            // Returns a random point in the camera defocus disc
            var p = RandomInUnitDisc();
            return center + (p.X * defocusDiscU) + (p.Y * defocusDiscV);
        }

        private Vector3 SampleSquare()
        {
            // Returns the vector to a random point in the [-.5,-.5]-[+.5,+.5] unit square.
            return new Vector3((float)(RandomDouble() - 0.5), (float)(RandomDouble() - 0.5), 0);
        }

        private static double LinearToGamma(double linearComponent)
        {
            if (linearComponent > 0.0)
            {
                return Math.Sqrt(linearComponent);
            }

            return 0.0;
        }

        private static readonly Interval ColorClampIntensity = new Interval(0.0, 0.999);
        private static string WriteColor(Vector3 color)
        {
            double r = color.X;
            double g = color.Y;
            double b = color.Z;

            // Apply a linear gamma transform for gamma 2
            r = LinearToGamma(r);
            g = LinearToGamma(g);
            b = LinearToGamma(b);

            // Translate the [0,1] component values to the byte range [0,255].

            int rbyte = (int)(256 * ColorClampIntensity.Clamp(r));
            int gbyte = (int)(256 * ColorClampIntensity.Clamp(g));
            int bbyte = (int)(256 * ColorClampIntensity.Clamp(b));

            return $"{rbyte} {gbyte} {bbyte}";
        }

        private void Initialize()
        {
            imageHeight = Math.Max(1, (int)(ImageWidth / AspectRatio));

            pixelSamplesScale = 1.0 / SamplesPerPixel;

            center = LookFrom;

            // determine viewport dimensions
            double focalLength = (LookFrom - LookAt).Length();
            double theta = DegreesToRadians(VFOV);
            double h = Math.Tan(theta / 2.0);
            double viewportHeight = 2.0f * h * FocusDist;
            double viewportWidth = (viewportHeight * ((double)(ImageWidth) / imageHeight));

            // Calculate the u,v,w unit basis vectors for the camera coordinate frame
            w = UnitVector(LookFrom - LookAt);
            u = UnitVector(Vector3.Cross(VUp, w));
            v = Vector3.Cross(w, u);

            // vectors across horizontal and down vertical viewport edges
            var viewportU = (float)viewportWidth * u;   // Vector across viewport horizontal edge
            var viewportV = (float)viewportHeight * -v; // Vector down viewport vertical edge

            // calculate the horizontal and vertical delta vectors from pixel to pixel
            pixelDeltaU = viewportU / ImageWidth;
            pixelDeltaV = viewportV / imageHeight;

            // location of upper-left pixel
            var viewportUpperLeft = center - ((float)FocusDist * w) - (viewportU / 2.0f) - (viewportV / 2.0f);
            pixel00Loc = viewportUpperLeft + 0.5f * (pixelDeltaU + pixelDeltaV);

            // Calculate the camera defocus disc basis vectors
            var defocusRadius = FocusDist * Math.Tan(DegreesToRadians(DefocusAngle / 2.0));
            defocusDiscU = u * (float)defocusRadius;
            defocusDiscV = v * (float)defocusRadius;
        }

        private Vector3 RayColor(Ray ray, int depth, Hittables world)
        {
            if (depth <= 0)
            {
                return new Vector3();
            }

            bool hit;
            HitRecord hitRecord = default;
            hit = world.Hit(ray, new Interval(0.0001, double.PositiveInfinity), ref hitRecord);

            // If ray hits nothing return the background color
            if (!hit)
            {
                return Background;
            }

            Ray? scattered;
            Vector3 attenuationColor;
            var colorFromEmission = hitRecord.Mat.Emitted(hitRecord.U, hitRecord.V, hitRecord.P);
            
            if (!hitRecord.Mat.Scatter(ray, hitRecord, out attenuationColor, out scattered))
            {
                return colorFromEmission;
            }
            var colorFromScatter = attenuationColor * RayColor(scattered.Value, depth - 1, world);
            return colorFromEmission + colorFromScatter;
        }
    }
}
