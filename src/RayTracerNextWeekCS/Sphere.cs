using System.Numerics;

namespace RayTracerNextWeekCS
{
    public class Sphere : IHittable
    {
        private readonly Ray centerRay;
        private readonly double radius;
        private readonly Material mat;

        private AABB bBox;

        public AABB BoundingBox => bBox;

        // Stationary sphere
        public Sphere(Vector3 center, double radius, Material mat)
        {
            this.centerRay = new Ray(center, new Vector3(0.0f, 0.0f, 0.0f));
            this.radius = Math.Max(0, radius);
            this.mat = mat;

            var rVec = new Vector3((float)radius, (float)radius, (float)radius);
            this.bBox = new AABB(center - rVec, center + rVec);
        }

        // Moving sphere
        public Sphere(Vector3 center1, Vector3 center2, double radius, Material mat)
        {
            this.centerRay = new Ray(center1, center2 - center1);
            this.radius = Math.Max(0, radius);
            this.mat = mat;

            var rvec = new Vector3((float)radius, (float)radius, (float)radius);
            var box1 = new AABB(center1 -rvec, center1 + rvec);
            var box2 = new AABB(center2 -rvec, center2 + rvec);
            this.bBox = new AABB(box1, box2);
        }

        public bool Hit(Ray ray, Interval rayT, ref HitRecord rec)
        {
            Vector3 centerAtT = centerRay.At((float)ray.Tm);
            Vector3 oc = centerAtT - ray.Origin;
            var a = ray.Direction.LengthSquared();
            var h = Vector3.Dot(ray.Direction, oc);
            var c = oc.LengthSquared() - radius * radius;

            var discriminant = h * h - a * c;
            if (discriminant < 0.0)
            {
                return false;
            }

            var sqrtd = Math.Sqrt(discriminant);

            // Find the nearest root that lies in the acceptable range
            var root = (h - sqrtd) / a;
            if (!rayT.Surrounds(root))
            {
                root = (h + sqrtd) / a;
                if (!rayT.Surrounds(root))
                {
                    return false;
                }
            }

            var point = ray.At((float)root);
            var outwardNormal = (point - centerAtT) / (float)radius;
            var (u, v) = GetSphereUV(outwardNormal);
            rec = new HitRecord(point, root, ray.Direction, outwardNormal, mat, u, v);

            return true;
        }

        private static (double, double) GetSphereUV(Vector3 p)
        {
            // p: a given point on the sphere of radius one, centered at the origin.
            // u: returned value [0,1] of angle around the Y axis from X=-1.
            // v: returned value [0,1] of angle from Y=-1 to Y=+1.
            //     <1 0 0> yields <0.50 0.50>       <-1  0  0> yields <0.00 0.50>
            //     <0 1 0> yields <0.50 1.00>       < 0 -1  0> yields <0.50 0.00>
            //     <0 0 1> yields <0.25 0.50>       < 0  0 -1> yields <0.75 0.50>

            var theta = Math.Acos(-p.Y);
            var phi = Math.Atan2(-p.Z, p.X) + Math.PI;

            var u = phi / (2 * Math.PI);
            var v = theta / Math.PI;
            return (u, v);
        }
    }
}
