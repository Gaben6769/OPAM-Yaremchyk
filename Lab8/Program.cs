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
