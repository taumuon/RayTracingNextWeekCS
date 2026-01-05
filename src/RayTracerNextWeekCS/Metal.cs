using System.Numerics;

namespace RayTracerNextWeekCS
{
    public class Metal : Material
    {
        private Vector3 albedoColor;
        private double fuzz;

        public Metal(Vector3 albedoColor, double fuzz)
        {
            this.albedoColor = albedoColor;
            this.fuzz = fuzz < 1 ? fuzz : 1;
        }

        public override bool Scatter(Ray rayIn, HitRecord hitRecord, out Vector3 attenuationColor, out Ray? scattered)
        {
            Vector3 reflected = ReflectVector(rayIn.Direction, hitRecord.Normal);
            reflected = UnitVector(reflected) + ((float)fuzz * RandomUnitVector());
            scattered = new Ray(hitRecord.P, reflected, rayIn.Tm);
            attenuationColor = albedoColor;
            return Vector3.Dot(scattered.Value.Direction, hitRecord.Normal) > 0;
        }
    }
}
