using System.Numerics;

namespace RayTracerNextWeekCS
{
    public readonly struct Ray
    {
        public Vector3 Origin { get; }
        public Vector3 Direction { get; }

        public double Tm { get; }

        public Ray(Vector3 origin, Vector3 direction, double tm = 0)
        {
            Origin = origin;
            Direction = direction;
            Tm = tm;
        }

        public Vector3 At(float t)
        {
            return Origin + t * Direction;
        }
    }
}
