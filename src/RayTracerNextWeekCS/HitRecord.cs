using System.Numerics;

namespace RayTracerNextWeekCS
{
    public readonly struct HitRecord
    {
        public Vector3 P { get; }
        public Vector3 Normal { get; }
        public double T { get; }

        public Material Mat { get; }

        public double U { get; }

        public double V { get; }
        public bool FrontFace { get; }

        public HitRecord(Vector3 p, double t, Vector3 normal, bool frontFace, Material mat, double u, double v)
        {
            P = p;
            T = t;
            Normal = normal;
            FrontFace = frontFace;
            Mat = mat;
            U = u;
            V = v;
        }

        public HitRecord(Vector3 p, double t, Vector3 rayDirection, Vector3 outwardNormal, Material mat, double u, double v)
        {
            P = p;
            T = t;
            FrontFace = Vector3.Dot(rayDirection, outwardNormal) < 0;
            Normal = FrontFace ? outwardNormal : -outwardNormal;
            Mat = mat;
            U = u;
            V = v;
        }

        public override string ToString()
        {
            return $"U:{U} V:{V} FrontFace:{FrontFace} T:{T} P:{P} Normal:{Normal} mat:{Mat?.GetType().Name}";
        }
    }
}
