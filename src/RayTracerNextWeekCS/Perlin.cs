using System.Numerics;

namespace RayTracerNextWeekCS
{
    public class Perlin
    {
        public Perlin()
        {
            randVec = new Vector3[PointCount];

            for (int i = 0; i < PointCount; ++i)
            {
                randVec[i] = UnitVector(RandomVector(-1.0, 1.0));
            }

            permX = PerlinGeneratePerm();
            permY = PerlinGeneratePerm();
            permZ = PerlinGeneratePerm();
        }

        static int[] PerlinGeneratePerm()
        {
            var p = Enumerable.Range(0, PointCount).ToArray();
            Permute(p, PointCount);
            return p;
        }

        static void Permute(Span<int> p, int n)
        {
            for (int i = n - 1; i > 0; i--)
            {
                int target = RandomInt(0, i);
                int tmp = p[i];
                p[i] = p[target];
                p[target] = tmp;
            }
        }

        public double Noise(in Vector3 p)
        {
            var u = p.X - Math.Floor(p.X);
            var v = p.Y - Math.Floor(p.Y);
            var w = p.Z - Math.Floor(p.Z);

            var i = (int)(Math.Floor(p.X));
            var j = (int)(Math.Floor(p.Y));
            var k = (int)(Math.Floor(p.Z));
            Span<Vector3> c = stackalloc Vector3[8];

            for (int di = 0; di < 2; di++)
                for (int dj = 0; dj < 2; dj++)
                    for (int dk = 0; dk < 2; dk++)
                        c[di * 4 + dj * 2 + dk] = randVec[
                            permX[(i + di) & 255] ^
                            permY[(j + dj) & 255] ^
                            permZ[(k + dk) & 255]
                        ];

            return PerlinInterp(c, u, v, w);
        }

        static double PerlinInterp(ReadOnlySpan<Vector3> c, double u, double v, double w)
        {
            var uu = u * u * (3 - 2 * u);
            var vv = v * v * (3 - 2 * v);
            var ww = w * w * (3 - 2 * w);
            var accum = 0.0;

            for (int i=0; i<2; i++)
                for (int j=0; j<2; j++)
                    for (int k=0; k<2; k++)
                        {
                            Vector3 weightV = new Vector3((float)u-i, (float)v-j, (float)w-k);
                            accum += (i * uu + (1-i)*(1-uu))
                               * (j* vv + (1-j)*(1-vv))
                               * (k* ww + (1-k)*(1-ww))
                               * Vector3.Dot(c[i * 4 + j * 2 + k], weightV);
                        }

            return accum;
        }

        public double Turb(Vector3 p, int depth)
        {
            var accum = 0.0;
            var tempP = p;
            var weight = 1.0;

            for (int i = 0; i<depth; i++)
            {
                accum += weight * Noise(tempP);
                weight *= 0.5;
                tempP *= 2;
            }
        
            return Math.Abs(accum);
        }

        private int[] permX;
        private int[] permY;
        private int[] permZ;
        private const int PointCount = 256;
        private readonly double[] RandFloat = new double[PointCount];
        private Vector3[] randVec;
    }
}
