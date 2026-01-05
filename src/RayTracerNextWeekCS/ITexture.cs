using System.Numerics;

namespace RayTracerNextWeekCS
{
    public interface ITexture
    {
        Vector3 ColorValue(double u, double v, Vector3 p);
    }
}
