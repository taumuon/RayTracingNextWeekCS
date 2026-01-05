namespace RayTracerNextWeekCS
{
    public class Hittables : IHittable
    {
        private List<IHittable> hittables = new List<IHittable>();
        private AABB bBox = AABB.Empty;

        public AABB BoundingBox => bBox;

        public List<IHittable> GetHittables() => hittables;

        public void Add(IHittable hittable)
        {
            hittables.Add(hittable);
            bBox = new AABB(bBox, hittable.BoundingBox);
        }

        public bool Hit(Ray ray, Interval rayT, ref HitRecord rec)
        {
            bool hitAnything = false;
            var closestSoFar = rayT.Max;

            foreach (var hittable in hittables)
            {
                bool hitResult;
                HitRecord hitRecord = default;
                hitResult = hittable.Hit(ray, new Interval(rayT.Min, closestSoFar), ref hitRecord);
                if (hitResult)
                {
                    hitAnything = true;
                    closestSoFar = hitRecord.T;
                    rec = hitRecord;
                }
            }

            return hitAnything;
        }
    }
}
