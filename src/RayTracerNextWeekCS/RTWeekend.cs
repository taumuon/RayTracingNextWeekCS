using System.Numerics;

namespace RayTracerNextWeekCS
{
    public static class RTWeekend
    {
        public static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private static readonly Random _random = Random.Shared;

        public static double RandomDouble()
        {
            // returns random number in [0, 1)
            return _random.NextDouble();
        }

        public static double RandomDouble(double min, double max)
        {
            return min + (max - min) * RandomDouble();
        }

        public static int RandomInt(int min, int max)
        {
            // Returns a random integer in [min,max].
            return (int)(RandomDouble(min, max + 1));
        }

        public static Vector3 RandomVector()
        {
            return new Vector3((float)RandomDouble(), (float)RandomDouble(), (float)RandomDouble());
        }

        public static Vector3 RandomVector(double min, double max)
        {
            return new Vector3((float)RandomDouble(min, max), (float)RandomDouble(min, max), (float)RandomDouble(min, max));
        }

        public static Vector3 UnitVector(Vector3 vector)
        {
            return vector / vector.Length();
        }

        public static Vector3 RandomInUnitDisc()
        {
            while (true)
            {
                var p = new Vector3((float)RandomDouble(-1.0, 1.0), (float)RandomDouble(-1.0, 1.0), 0.0f);
                if (p.LengthSquared() < 1)
                    return p;
            }
        }

        public static Vector3 RandomUnitVector()
        {
            while (true)
            {
                var p = RandomVector(-1, 1);
                var lengthSquared = p.LengthSquared();
                if (1e-6 < lengthSquared && lengthSquared <= 1)
                {
                    return p * (float)(1.0 / Math.Sqrt(lengthSquared));
                }
            }
        }


        public static bool VectorNearZero(Vector3 vector)
        {
            const double s = 1e-8;
            return Math.Abs(vector.X) < s && Math.Abs(vector.Y) < s && Math.Abs(vector.Z) < s;
        }

        public static Vector3 ReflectVector(Vector3 v, Vector3 n)
        {
            return v - 2.0f * Vector3.Dot(v, n) * n;
        }

        public static Vector3 RefractVector(Vector3 uv, Vector3 n, double etaiOverEtat)
        {
            var cosTheta = Math.Min(Vector3.Dot(-uv, n), 1.0);
            Vector3 rOutPerp = (float)etaiOverEtat * (uv + (float)cosTheta * n);
            Vector3 rOutParallel = (float)-Math.Sqrt(Math.Abs(1.0 - rOutPerp.LengthSquared())) * n;
            return rOutPerp + rOutParallel;
        }
    }
}
