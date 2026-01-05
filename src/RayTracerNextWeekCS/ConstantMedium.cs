using System.Numerics;

namespace RayTracerNextWeekCS
{
    public class ConstantMedium : IHittable
    {
        public ConstantMedium(IHittable boundary, double density, ITexture tex)
        {
            this.boundary = boundary;
            negInvDensity = -1.0 / density;
            phaseFunction = new Isotropic(tex);
        }

        public ConstantMedium(IHittable boundary, double density, Vector3 albedoColor)
        {
            this.boundary = boundary;
            negInvDensity = -1.0 / density;
            phaseFunction = new Isotropic(albedoColor);
        }

        public AABB BoundingBox => boundary.BoundingBox;

        public bool Hit(Ray ray, Interval rayT, ref HitRecord rec)
        {
            HitRecord rec1 = default, rec2 = default;

            if (!boundary.Hit(ray, Interval.Universe, ref rec1))
            {
                return false;
            }

            if (!boundary.Hit(ray, new Interval(rec1.T + 0.0001, double.PositiveInfinity), ref rec2))
            {
                return false;
            }

            var rec1T = rec1.T;
            var rec2T = rec2.T;
            if (rec1T < rayT.Min) rec1T = rayT.Min;
            if (rec2T > rayT.Max) rec2T = rayT.Max;

            if (rec1T >= rec2T)
            {
                return false;
            }

            if (rec1T < 0)
                rec1T = 0;

            var ray_length = ray.Direction.Length();
            var distance_inside_boundary = (rec2T - rec1T) * ray_length;
            var hit_distance = negInvDensity * Math.Log(RandomDouble());

            if (hit_distance > distance_inside_boundary)
            {
                return false;
            }

            var t = rec1T + hit_distance / ray_length;
            var p = ray.At((float)t);
            rec = new HitRecord(p,
                t,
                new Vector3(1.0f, 0.0f, 0.0f), // arbitrary,
                true, // also arbitrary,
                phaseFunction,
                0.0,
                0.0);

            return true;
        }

        IHittable boundary;
        double negInvDensity;
        Material phaseFunction;
    }
}
