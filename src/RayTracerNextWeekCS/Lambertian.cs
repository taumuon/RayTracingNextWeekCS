using System.Numerics;

namespace RayTracerNextWeekCS
{
    public class Lambertian : Material
    {
        private ITexture tex;

        public Lambertian(Vector3 albedoColor)
        {
            tex = new SolidColorTexture(albedoColor);
        }
        public Lambertian(ITexture tex)
        {
            this.tex = tex;
        }

        public override bool Scatter(Ray rayIn, HitRecord hitRecord, out Vector3 attenuationColor, out Ray? scattered)
        {
            var scatterDirection = hitRecord.Normal + RandomUnitVector();

            // Catch degenerate scatter direction
            if (VectorNearZero(scatterDirection))
            {
                scatterDirection = hitRecord.Normal;
            }

            scattered = new Ray(hitRecord.P, scatterDirection, rayIn.Tm);
            attenuationColor = tex.ColorValue(hitRecord.U, hitRecord.V, hitRecord.P);
            return true;
        }
    }
}
