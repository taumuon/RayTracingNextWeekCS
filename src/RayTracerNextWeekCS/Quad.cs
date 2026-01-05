using System.Numerics;

namespace RayTracerNextWeekCS
{
    public class Quad : IHittable
    {
        public Quad(Vector3 Q, Vector3 u, Vector3 v, Material mat)
        {
            this.Q = Q;
            this.u = u;
            this.v = v;
            this.mat = mat;

            var n = Vector3.Cross(u, v);
            normal = UnitVector(n);
            D = Vector3.Dot(normal, Q);

            w = n / Vector3.Dot(n, n);

            SetBoundingBox();
        }

        private void SetBoundingBox()
        {
            // Compute the bounding box of all four vertices.
            var bbox_diagonal1 = new AABB(Q, Q + u + v);
            var bbox_diagonal2 = new AABB(Q + u, Q + v);
            bBox = new AABB(bbox_diagonal1, bbox_diagonal2);
        }

        public AABB BoundingBox => bBox;

        public bool Hit(Ray ray, Interval rayT, ref HitRecord rec)
        {
            var denom = Vector3.Dot(normal, ray.Direction);

            // No hit if the ray is parallel to the plane.
            if (Math.Abs(denom) < 1e-8)
            {
                return false;
            }

            // Return false if the hit point parameter t is outside the ray interval.
            var t = (D - Vector3.Dot(normal, ray.Origin)) / denom;
            if (!rayT.Contains(t))
            {
                return false;
            }

            // Determine the hit point lies within the planar shape using its plane coordinates.
            var intersection = ray.At((float)t);

            Vector3 planarHitPtVector = intersection - Q;
            var alpha = Vector3.Dot(w, Vector3.Cross(planarHitPtVector, v));
            var beta = Vector3.Dot(w, Vector3.Cross(u, planarHitPtVector));

            if (!IsInterior(alpha, beta, intersection, t, ray.Direction, normal, mat, ref rec))
            {
                return false;
            }

            return true;
        }

        static bool IsInterior(double a, double b, Vector3 intersection, double t, Vector3 rayDirection, Vector3 normal, Material mat, ref HitRecord rec) 
        {
            Interval unitInterval = new Interval(0, 1);
            // Given the hit point in plane coordinates, return false if it is outside the primitive

            if (!unitInterval.Contains(a) || !unitInterval.Contains(b))
            {
                return false;
            }

            rec = new HitRecord(intersection, t, rayDirection, normal, mat, a, b);
            return true;
        }

        Vector3 Q;
        Vector3 u, v;
        Material mat;
        AABB bBox;
        Vector3 normal;
        double D;
        Vector3 w;
    }
}
