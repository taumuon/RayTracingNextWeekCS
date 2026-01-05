using System.Numerics;

namespace RayTracerNextWeekCS
{
    public class CheckerTexture : ITexture
    {
        private readonly double invScale;
        private readonly ITexture even;
        private readonly ITexture odd;

        public CheckerTexture(double scale, ITexture even, ITexture odd)
        {
            invScale = 1.0 / scale;
            this.even = even;
            this.odd = odd;
        }

        public CheckerTexture(double scale, Vector3 color1, Vector3 color2)
        {
            invScale = 1.0 / scale;
            this.even = new SolidColorTexture(color1);
            this.odd = new SolidColorTexture(color2);
        }

        public Vector3 ColorValue(double u, double v, Vector3 p)
        {
            var uInteger = (int)Math.Floor(invScale * u);
            var vInteger = (int)Math.Floor(invScale * v);

            var isEven = (uInteger + vInteger) % 2 == 0;

            return isEven ? even.ColorValue(u, v, p) : odd.ColorValue(u, v, p);
        }
    }
}
