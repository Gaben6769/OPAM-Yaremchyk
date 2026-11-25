using System.Diagnostics;

static class Measurements
{
    public static double Run<T>(Func<T> measure)
    {
        GC.KeepAlive(measure());
        double best = double.PositiveInfinity;
        for (int i = 0; i < 3; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            T result = measure();
            stopwatch.Stop();
            GC.KeepAlive(result);
            best = Math.Min(best, stopwatch.Elapsed.TotalMilliseconds);
        }
        return best;
    }
}
