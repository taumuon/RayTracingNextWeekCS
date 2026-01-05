using System.Numerics;

namespace RayTracerNextWeekCS
{
    public class DiffuseLight : Material
    {
        public DiffuseLight(ITexture tex)
        {
            this.tex = tex;
        }

        public DiffuseLight(Vector3 color)
        {
            this.tex = new SolidColorTexture(color);
        }

        public override Vector3 Emitted(double u, double v, Vector3 p)
        {
            return tex.ColorValue(u, v, p);
        }

        ITexture tex;
    }
}
