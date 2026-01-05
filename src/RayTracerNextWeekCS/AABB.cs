using System.Numerics;

namespace RayTracerNextWeekCS
{
    [System.Diagnostics.DebuggerDisplay("X: {X} Y: {Y} Z: {Z}")]
    public class AABB
    {
        public Interval X { get; }

        public Interval Y { get; }

        public Interval Z { get; }

        public static AABB Empty { get; } = new AABB(Interval.Empty, Interval.Empty, Interval.Empty);

        public AABB(Vector3 a, Vector3 b)
        {
            // Treat the two points a and b as extrema for the bounding box, so we don't require a
            // particular minimum/maximum coordinate order.

            var x = (a[0] <= b[0]) ? new Interval(a[0], b[0]) : new Interval(b[0], a[0]);
            var y = (a[1] <= b[1]) ? new Interval(a[1], b[1]) : new Interval(b[1], a[1]);
            var z = (a[2] <= b[2]) ? new Interval(a[2], b[2]) : new Interval(b[2], a[2]);

            var delta = 0.0001;
            if (x.Size < delta) { x = x.Expand(delta); }
            if (y.Size < delta) { y = y.Expand(delta); }
            if (z.Size < delta) { z = z.Expand(delta); }
            X = x;
            Y = y;
            Z = z;
        }

        public AABB(Interval x, Interval y, Interval z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public AABB(AABB box0, AABB box1)
        {
            var x = new Interval(box0.X, box1.X);
            var y = new Interval(box0.Y, box1.Y);
            var z = new Interval(box0.Z, box1.Z);

            var delta = 0.0001;
            if (x.Size < delta) { x = x.Expand(delta); }
            if (y.Size < delta) { y = y.Expand(delta); }
            if (z.Size < delta) { z = z.Expand(delta); }
            X = x;
            Y = y;
            Z = z;
        }

        public Interval AxisInterval(int n)
        {
            if (n == 0) return X;
            if (n == 1) return Y;
            return Z;
        }

        public double AxisIntervalMin(int n)
        {
            if (n == 0) return X.Min;
            if (n == 1) return Y.Min;
            return Z.Min;
        }

        public bool Hit(Ray r, Interval rayT)
        {
            Vector3 rayOrigin = r.Origin;
            Vector3 rayDirection = r.Direction;

            var intervalMin = rayT.Min;
            var intervalMax = rayT.Max;

            for (int axis = 0; axis < 3; axis++)
            {
                Interval ax = AxisInterval(axis);
                double adInv = 1.0 / rayDirection[axis];

                var rayOriginAxis = rayOrigin[axis];
                var t0 = (ax.Min - rayOriginAxis) * adInv;
                var t1 = (ax.Max - rayOriginAxis) * adInv;

                if (t0 < t1)
                {
                    if (t0 > intervalMin) intervalMin = t0;
                    if (t1 < intervalMax) intervalMax = t1;
                }
                else
                {
                    if (t1 > intervalMin) intervalMin = t1;
                    if (t0 < intervalMax) intervalMax = t0;
                }

                if (intervalMax <= intervalMin)
                    return false;
            }

            return true;
        }

        public int LongestAxis
        {
            get
            {
                if (X.Size > Y.Size)
                {
                    return X.Size > Z.Size ? 0 : (Z.Size > Y.Size ? 2 : 1);
                }
                else
                {
                    return Y.Size > Z.Size ? 1 : (Z.Size > X.Size ? 2 : 0);
                }
            }
        }

        public static AABB operator +(AABB bBox, Vector3 offset) => new AABB(bBox.X + offset.X, bBox.Y + offset.Y, bBox.Z + offset.Z);

        public override string ToString()
        {
            return $"X:{X} Y:{Y} Z:{Z}";
        }
    }
}
