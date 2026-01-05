using System.Numerics;

namespace RayTracerNextWeekCS
{
    public abstract class Material
    {
        public virtual bool Scatter(Ray rayIn, HitRecord hitRecord, out Vector3 attenuationColor, out Ray? scattered)
        {
            scattered = null;
            attenuationColor = new Vector3();
            return false;
        }

        public virtual Vector3 Emitted(double u, double v, Vector3 p)
        {
            return new Vector3();
        }
    }
}
