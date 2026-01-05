using System.Numerics;

namespace RayTracerNextWeekCS
{
    public class Translate : IHittable
    {
        public Translate(IHittable hittable, Vector3 offset)
        {
            this.hittable = hittable;
            this.offset = offset;

            this.bBox = hittable.BoundingBox + offset;
        }

        public AABB BoundingBox => bBox;

        public bool Hit(Ray ray, Interval rayT, ref HitRecord rec)
        {
            // Move the ray backwards by the offset
            var offsetR = new Ray(ray.Origin - offset, ray.Direction, ray.Tm);

            HitRecord r = default;
            // Determine whether an intersection exists along the offset ray (and if so, where)
            if (!hittable.Hit(offsetR, rayT, ref r))
            {
                rec = r;
                return false;
            }

            var recValue = r;
            rec = new HitRecord(recValue.P + offset, // Move the intersection point forwards by the offset
                recValue.T,
                recValue.Normal,
                recValue.FrontFace,
                recValue.Mat,
                recValue.U,
                recValue.V);

            return true;
        }

        IHittable hittable;
        Vector3 offset;
        AABB bBox;
    }
}
