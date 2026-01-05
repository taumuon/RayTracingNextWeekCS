namespace RayTracerNextWeekCS
{
    public class Stats
    {
        public Stats(long allocatedBytes, long elapsedMilliseconds)
        {
            AllocatedBytes = allocatedBytes;
            ElapsedMilliseconds = elapsedMilliseconds;
        }

        public long AllocatedBytes { get; }

        public long ElapsedMilliseconds { get; }
    }
}
