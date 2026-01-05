using System.Numerics;

namespace RayTracerNextWeekCS
{
    public class Isotropic : Material
    {
        public Isotropic(ITexture tex)
        {
            this.tex = tex;
        }

        public Isotropic(Vector3 colorAlbedo)
        {
            this.tex = new SolidColorTexture(colorAlbedo);
        }

        public override bool Scatter(Ray rayIn, HitRecord hitRecord, out Vector3 attenuationColor, out Ray? scattered)
        {
            scattered = new Ray(hitRecord.P, RandomUnitVector(), rayIn.Tm);
            attenuationColor = tex.ColorValue(hitRecord.U, hitRecord.V, hitRecord.P);
            return true;
        }

        ITexture tex;
    }
}
