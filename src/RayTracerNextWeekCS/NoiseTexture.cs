using System.Numerics;

namespace RayTracerNextWeekCS
{
    public class NoiseTexture : ITexture
    {
        public NoiseTexture(double scale)
        {
            this.scale = scale;
        }

        public Vector3 ColorValue(double u, double v, Vector3 p)
        {
            // return new Vector3(1, 1, 1) * 0.5f * (float)(1.0 + noise.Noise((float)scale * p));
            // return new Vector3(1, 1, 1) * (float)noise.Turb(p, 7);
            return new Vector3(0.5f, 0.5f, 0.5f) * (float)(1.0 + Math.Sin(scale * p.Z + 10 * noise.Turb(p, 7)));
        }

        Perlin noise = new Perlin();
        readonly double scale;
    }
}
