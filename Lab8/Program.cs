#:property PublishAot=false

using System.Diagnostics;
using System.Globalization;
using static Search;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("N = 31, K = 6, a = 1, b = 3, c = 1");
string x = "Д-00", y = "Д-99", missing = "Я-99";
var original = MakeValues(100000);
var values = (string[])original.Clone();
Array.Sort(values, Compare);

Console.WriteLine("| Запит | Лінійний індекс / порівняння | Бінарний | Без рівності в циклі |");
Console.WriteLine("|---|---|---|---|");
foreach (string target in new[] { x, values[0], missing })
{
    int linear = LinearSearch(values, target, Compare, out int lc);
    int binary = BinarySearch(values, target, Compare, out int bc);
    int final = BinaryWithoutEquality(values, target, Compare, out int fc);
    if ((linear >= 0) != (binary >= 0) || linear != final || (binary >= 0 && values[binary] != target))
        throw new InvalidOperationException(target);
    Console.WriteLine($"| {target} | {linear} / {lc} | {binary} / {bc} | {final} / {fc} |");
}

int failed = 0;
foreach (var target in MakeQueries(original, 100))
    if (Broken.SearchUnsorted(original, target, Compare) < 0)
        failed++;
Console.WriteLine($"1. Останній кандидат: {Broken.SkipLast(new[] { 1, 2 }, 2, CompareInt)}.");
Console.WriteLine($"2. Межі після 10 кроків: {Broken.Stuck(new[] { 1, 2 }, 2, CompareInt)}.");
Console.WriteLine($"3. Переповнення: {Broken.OverflowingMidpoint(1073741824, 1073741824)}.");
Console.WriteLine($"4. Невдалі пошуки без сортування: {failed}/100.");

int left = Boundary(values, s => Compare(s, x) >= 0, out int cl);
int right = Boundary(values, s => Compare(s, x) > 0, out int cr);
int upper = Boundary(values, s => Compare(s, y) > 0, out int cu);
int linearEqual = 0, linearRange = 0, rangeComparisons = 0;
foreach (var code in values)
{
    int lowOrder = Compare(code, x);
    if (lowOrder == 0)
        linearEqual++;
    rangeComparisons++;
    if (lowOrder >= 0)
    {
        rangeComparisons++;
        if (Compare(code, y) <= 0)
            linearRange++;
    }
}
if (right - left != linearEqual || upper - left != linearRange)
    throw new InvalidOperationException("Межі не збігаються з перебором.");
Console.WriteLine($"Дорівнюють X: {right-left}, порівняння {cl+cr} проти {values.Length}.");
Console.WriteLine($"Належать [X; Y]: {upper-left}, порівняння {cl+cu} проти {rangeComparisons}.");
int badBoundary = Boundary(values, HasSeven, out _);
int firstSeven = 0;
while (firstSeven < values.Length && !HasSeven(values[firstSeven]))
    firstSeven++;
Console.WriteLine($"Код із сімкою: межа {badBoundary}, справжній індекс {firstSeven}.");

Console.WriteLine("| Вхід | Запит | Очікувано | Отримано |");
Console.WriteLine("|---|---|---|---|");
Check("порожній", [], 5, "-1 / 0 / 0");
Check("один", [5], 5, "0 / 0 / 1");
Check("два, шуканий другий", [1, 2], 2, "1 / 1 / 2");
Check("менше за всі", [2, 3], 1, "-1 / 0 / 0");
Check("більше за всі", [2, 3], 4, "-1 / 2 / 2");
Check("усі рівні", [5, 5, 5], 5, "1 / 0 / 3");
Console.WriteLine();
Measurements.Run(MakeQueries, MakeValues, Compare);

static int Compare(string a, string b) => a.CompareTo(b);
static int CompareInt(int a, int b) => a.CompareTo(b);
static bool HasSeven(string s) => s[2] == '7' || s[3] == '7';

static void Check(string name, int[] input, int target, string expected)
{
    int index = BinarySearch(input, target, CompareInt, out _);
    int lo = Boundary(input, n => n >= target, out _);
    int hi = Boundary(input, n => n > target, out _);
    string actual = $"{index} / {lo} / {hi}";
    if (actual != expected)
        throw new InvalidOperationException(name);
    Console.WriteLine($"| {name} | {target} | {expected} | {actual} |");
}

static string[] MakeValues(int size)
{
    const string alphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ";
    var random = new Random(31);
    var result = new string[size];
    for (int i = 0; i < size; i++)
    {
        string code;
        do
        {
            code = $"{alphabet[random.Next(alphabet.Length)]}-{random.Next(100):D2}";
        } while (code == "Я-99");
        result[i] = code;
    }
    return result;
}

static string[] MakeQueries(string[] values, int count)
{
    var random = new Random(31);
    var result = new string[count];
    for (int i = 0; i < count; i++)
        result[i] = values[random.Next(values.Length)];
    return result;
}

static class Broken
{
    public static int SkipLast<T>(T[] values, T target, Func<T, T, int> cmp)
    {
        int lo = 0, hi = values.Length - 1;
        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2;
            int order = cmp(values[mid], target);
            if (order == 0)
                return mid;
            if (order < 0)
                lo = mid + 1;
            else
                hi = mid - 1;
        }
        return -1;
    }

    public static (int Lo, int Hi) Stuck<T>(T[] values, T target, Func<T, T, int> cmp)
    {
        int lo = 0, hi = values.Length - 1;
        for (int step = 0; lo <= hi && step < 10; step++)
        {
            int mid = lo + (hi - lo) / 2;
            int order = cmp(values[mid], target);
            if (order == 0)
                return (mid, mid);
            if (order < 0)
                lo = mid;
            else
                hi = mid - 1;
        }
        return (lo, hi);
    }

    public static int OverflowingMidpoint(int lo, int hi) => unchecked((lo + hi) / 2);

    public static int SearchUnsorted<T>(T[] values, T target, Func<T, T, int> cmp) =>
        Search.BinarySearch(values, target, cmp, out _);
}

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

static class Search
{
    public static int LinearSearch<T>(T[] values, T target, Func<T, T, int> cmp, out int comparisons)
    {
        comparisons = 0;
        for (int i = 0; i < values.Length; i++)
        {
            comparisons++;
            if (cmp(values[i], target) == 0)
                return i;
        }
        return -1;
    }

    public static int BinarySearch<T>(T[] values, T target, Func<T, T, int> cmp, out int comparisons)
    {
        comparisons = 0;
        int lo = 0, hi = values.Length - 1;
        while (lo <= hi)
        {
            int mid = lo + (hi - lo) / 2;
            int order = cmp(values[mid], target);
            comparisons++;
            if (order == 0)
                return mid;
            if (order < 0)
                lo = mid + 1;
            else
                hi = mid - 1;
        }
        return -1;
    }

    public static int Boundary<T>(T[] values, Func<T, bool> predicate, out int comparisons)
    {
        comparisons = 0;
        int lo = 0, hi = values.Length;
        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2;
            comparisons++;
            if (predicate(values[mid]))
                hi = mid;
            else
                lo = mid + 1;
        }
        return lo;
    }

    public static int BinaryWithoutEquality<T>(T[] values, T target, Func<T, T, int> cmp,
        out int comparisons)
    {
        int pos = Boundary(values, value => cmp(value, target) >= 0, out comparisons);
        if (pos == values.Length)
            return -1;
        comparisons++;
        return cmp(values[pos], target) == 0 ? pos : -1;
    }
}
