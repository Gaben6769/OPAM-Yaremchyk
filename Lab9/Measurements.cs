using System.Diagnostics;

delegate void Sort<T>(T[] values, Func<T, T, int> cmp, out int comparisons, out int moves);

static class Measurements
{
    public static double Measure<T>(T[] source, Func<T, T, int> cmp, Sort<T> sort) where T : notnull
    {
        T[] copy = (T[])source.Clone();
        sort(copy, cmp, out _, out _);
        Checks.Validate(copy, source, cmp);

        double best = double.PositiveInfinity;
        for (int i = 0; i < 3; i++)
        {
            copy = (T[])source.Clone();
            var stopwatch = Stopwatch.StartNew();
            sort(copy, cmp, out _, out _);
            stopwatch.Stop();
            best = Math.Min(best, stopwatch.Elapsed.TotalMilliseconds);
            Checks.Validate(copy, source, cmp);
        }
        return best;
    }
}
