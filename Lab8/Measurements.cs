using System.Diagnostics;

static class Measurements
{
    public static void Run(Func<string[], int, string[]> makeQueries, Func<int, string[]> makeValues,
        Func<string, string, int> cmp)
    {
        Console.WriteLine("| n | Лінійний | Бінарний | Відношення | n / 2 та log₂ n |");
        Console.WriteLine("|---:|---:|---:|---:|---:|");
        foreach (int size in new[] { 1000, 10000, 100000, 1000000 })
        {
            var values = makeValues(size);
            var sorted = (string[])values.Clone();
            Array.Sort(sorted, (x, y) => cmp(x, y));
            int linear = 0, binary = 0;
            foreach (var target in makeQueries(values, 100))
            {
                Search.LinearSearch(values, target, cmp, out int a);
                Search.BinarySearch(sorted, target, cmp, out int b);
                linear += a;
                binary += b;
            }
            Console.WriteLine($"| {size} | {linear / 100d:F2} | {binary / 100d:F2} | {(double)linear / binary:F2} | {size / 2} / {Math.Ceiling(Math.Log2(size))} |");
        }

        var original = makeValues(100000);
        Console.WriteLine();
        Console.WriteLine("| q | Лінійно, мс | Сортування й бінарно, мс | Швидше |");
        Console.WriteLine("|---:|---:|---:|---|");
        int first = 0;
        foreach (int q in new[] { 1, 5, 10, 20, 30, 50, 100, 200, 400, 800, 1600 })
        {
            var queries = makeQueries(original, q);
            double linear = Measure(() =>
            {
                foreach (var target in queries)
                    Search.LinearSearch(original, target, cmp, out _);
            });
            double sorted = Measure(() =>
            {
                var copy = (string[])original.Clone();
                Array.Sort(copy, (x, y) => cmp(x, y));
                foreach (var target in queries)
                    Search.BinarySearch(copy, target, cmp, out _);
            });
            if (first == 0 && sorted < linear)
                first = q;
            Console.WriteLine($"| {q} | {linear:F3} | {sorted:F3} | {(sorted < linear ? "сортування" : "лінійно")} |");
        }

        Console.WriteLine(first == 0
            ? "На перевірених q до 1600 точку окупності не досягнуто."
            : $"Перша перевага сортування серед виміряних точок: q = {first}.");
    }

    private static double Measure(Action run)
    {
        run();
        double best = double.PositiveInfinity;
        for (int i = 0; i < 3; i++)
        {
            var timer = Stopwatch.StartNew();
            run();
            timer.Stop();
            best = Math.Min(best, timer.Elapsed.TotalMilliseconds);
        }
        return best;
    }
}

