namespace RayTracerNextWeekCS
{
    public interface IHittable
    {
        bool Hit(Ray ray, Interval rayT, ref HitRecord rec);

        AABB BoundingBox { get; }
    }
}
