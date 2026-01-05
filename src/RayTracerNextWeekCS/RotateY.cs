using System.Numerics;

namespace RayTracerNextWeekCS
{
    public class RotateY : IHittable
    {
        public RotateY(IHittable hittable, double angle)
        {
            this.hittable = hittable;
            var radians = DegreesToRadians(angle);
            sinTheta = Math.Sin(radians);
            cosTheta = Math.Cos(radians);
            bBox = hittable.BoundingBox;

            var min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            var max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        var x = i * bBox.X.Max + (1 - i) * bBox.X.Min;
                        var y = j * bBox.Y.Max + (1 - j) * bBox.Y.Min;
                        var z = k * bBox.Z.Max + (1 - k) * bBox.Z.Min;

                        var newX = cosTheta * x + sinTheta * z;
                        var newZ = -sinTheta * x + cosTheta * z;

                        var tester = new Vector3((float)newX, (float)y, (float)newZ);

                        for (int c = 0; c < 3; c++)
                        {
                            min[c] = Math.Min(min[c], tester[c]);
                            max[c] = Math.Max(max[c], tester[c]);
                        }
                    }
                }
            }

            bBox = new AABB(min, max);
        }

        public AABB BoundingBox => bBox;

        public bool Hit(Ray ray, Interval rayT, ref HitRecord rec)
        {
            // Change the ray from world space to object space
            var newOrigin = new Vector3(
                (float)(cosTheta * ray.Origin[0] - sinTheta * ray.Origin[2]),
                ray.Origin[1],
                (float)(sinTheta * ray.Origin[0] + cosTheta * ray.Origin[2])
                );

            var newDirection = new Vector3(
                (float)(cosTheta * ray.Direction[0] - sinTheta * ray.Direction[2]),
                ray.Direction[1],
                 (float)(sinTheta * ray.Direction[0] + cosTheta * ray.Direction[2])
                );

            var rotatedR = new Ray(newOrigin, newDirection, ray.Tm);

            HitRecord r = default;
            // Determine whether an intersection exists in object space (and if so, where)
            if (!hittable.Hit(rotatedR, rayT, ref r))
            {
                return false;
            }

            var recValue = r;
            // Change the intersection point from object space to world space
            var p = new Vector3(
                (float)(cosTheta * recValue.P[0] + sinTheta * recValue.P[2]),
                recValue.P[1],
                (float)(-sinTheta * recValue.P[0] + cosTheta * recValue.P[2])
                );

            // Change the normal from object space to world space
            var normal = new Vector3(
                (float)(cosTheta * recValue.Normal[0] + sinTheta * recValue.Normal[2]),
                recValue.Normal[1],
                (float)(-sinTheta * recValue.Normal[0] + cosTheta * recValue.Normal[2])
                );

            rec = new HitRecord(p,
                recValue.T,
                normal,
                recValue.FrontFace,
                recValue.Mat,
                recValue.U,
                recValue.V);

            return true;
        }

        private double sinTheta;
        private double cosTheta;
        private AABB bBox;
        private IHittable hittable;
    }
}
