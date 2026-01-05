namespace RayTracerNextWeekCS
{
    [System.Diagnostics.DebuggerDisplay("min:{Min} max:{Max}")]
    public readonly struct Interval
    {
        public double Min { get; }

        public double Max { get; }

        public Interval() :this (double.PositiveInfinity, double.NegativeInfinity) { }

        public Interval(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public Interval(Interval a, Interval b)
        {
            // Create the interval tightly enclosing the two input intervals.
            Min = Math.Min(a.Min, b.Min);
            Max = Math.Max(a.Max, b.Max);
        }

        public double Size => Max - Min;

        public bool Contains(double x) => Min <= x && x <= Max;

        public bool Surrounds(double x) => Min < x && x < Max;

        public Interval Expand(double delta)
        {
            var padding = delta / 2.0;
            return new Interval(Min - padding, Min + padding);
        }

        public static Interval Empty { get; } = new Interval();

        public static Interval Universe { get; } = new Interval(double.NegativeInfinity, double.PositiveInfinity);

        public double Clamp(double x)
        {
            if (x < Min) { return Min; }
            if (x > Max) { return Max; }
            return x;
        }

        public static Interval operator +(Interval interval, double displacement) => new Interval(interval.Min + displacement, interval.Max + displacement);

        public override string ToString()
        {
            return $"min:{Min} max:{Max}";
        }
    }
}
