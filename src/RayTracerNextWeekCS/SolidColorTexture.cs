using System.Numerics;

namespace RayTracerNextWeekCS
{
    public class SolidColorTexture : ITexture
    {
        private readonly Vector3 albedo;

        public SolidColorTexture(Vector3 albedo)
        {
            this.albedo = albedo;
        }

        public Vector3 ColorValue(double u, double v, Vector3 p)
        {
            return albedo;
        }
    }
}
