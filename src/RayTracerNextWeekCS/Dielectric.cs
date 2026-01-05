using System.Numerics;

namespace RayTracerNextWeekCS
{
    public class Dielectric : Material
    {
        // Refractive index in vacuum or air, or the ratio of the material's refractive index over
        // the refractive index of the enclosing media
        private double refractionIndex;

        public Dielectric(double refractionIndex)
        {
            this.refractionIndex = refractionIndex;
        }

        public override bool Scatter(Ray rayIn, HitRecord hitRecord, out Vector3 attenuationColor, out Ray? scattered)
        {
            attenuationColor = new Vector3(1.0f, 1.0f, 1.0f);
            double ri = hitRecord.FrontFace ? (1.0 / refractionIndex) : refractionIndex;

            Vector3 unitDirection = UnitVector(rayIn.Direction);

            double cosTheta = Math.Min(Vector3.Dot(-unitDirection, hitRecord.Normal), 1.0);
            double sinTheta = Math.Sqrt(1.0 - cosTheta * cosTheta);

            bool cannotRefract = ri * sinTheta > 1.0;
            Vector3 direction;

            if (cannotRefract || Reflectance(cosTheta, ri) > RandomDouble())
            {
                direction = ReflectVector(unitDirection, hitRecord.Normal);
            }
            else
            {
                direction = RefractVector(unitDirection, hitRecord.Normal, ri);
            }

            scattered = new Ray(hitRecord.P, direction, rayIn.Tm);
            return true;
        }

        private static double Reflectance(double cosine, double refractionIndex)
        {
            // Use Schlick's approximation for reflectance
            var r0 = (1.0 - refractionIndex) / (1.0 + refractionIndex);
            r0 = r0 * r0;
            return r0 + (1.0 - r0) * Math.Pow((1.0 - cosine), 5.0);
        }
    }
}
