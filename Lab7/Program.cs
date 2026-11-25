#:property PublishAot=false

using System.Numerics;
using static Algorithms;

const int N = 31;
const int K = 6;
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("N = 31, K = 6, a = 1, b = 3, c = 1");

var random = new Random(N);
var numbers = new List<int>();
for (int i = 0; i < 20; i++)
    numbers.Add(random.Next(-K, K + 1));

var words = new List<string>
{
    "око", "алгоритм", "дерево", "пів-Європи", "їжак", "чай", "ґанок",
    "синьо-жовтий", "ім'я", "Львів", "функція", "елемент"
};
var students = Students.FromLab6();

Print("Усі числа", numbers);
var multiples = Filter(numbers, n => n % 3 == 0);
Print("Кратні трьом", multiples);
Print("Починаються з голосної", Filter(words, w => IsVowel(w[0])));
Print("Група КН-3/1", Filter(students, s => s.Group == "КН-3/1"));

Print("Числа в дужках", Map(numbers, n => $"({n})"));
Print("Кількість голосних", Map(words, CountVowels));
Print("Чи склав", Map(students, s => s.Score >= 60));
Print("Кратні трьом у дужках", Map(multiples, n => $"({n})"));

Console.WriteLine($"Сума: {Aggregate(numbers, 0, (sum, n) => sum + n)}");
Console.WriteLine($"Добуток: {Aggregate(numbers, BigInteger.One, (product, n) => product * n)}");
Console.WriteLine($"Максимум: {Maximum(numbers)}");
Console.WriteLine($"Найбільший за модулем: {Aggregate(numbers, 0, (best, n) =>
    Math.Abs(n) > Math.Abs(best) ? n : best)}");

PrintGroups("За групою", GroupBy(students, s => s.Group));
PrintGroups("За першою літерою", GroupBy(students, s => s.Name[0]));
PrintGroups("За балом", GroupBy(students, s => s.Score switch
{
    < 60 => "0–59", < 75 => "60–74", < 90 => "75–89", _ => "90–100"
}));
PrintGroups("Довжина слова", GroupBy(words, w => w.Length));

int limit = K;
Func<int, bool> overLimit = n => n > limit;
bool before = overLimit(K + 1);
limit = 1000;
Console.WriteLine($"Поріг: {before} -> {overLimit(K + 1)}");
var actions = new List<Func<int>>();
for (int i = 0; i < 3; i++)
    actions.Add(() => i);
Print("for", Map(actions, action => action()));
actions.Clear();
foreach (int i in new[] { 0, 1, 2 })
    actions.Add(() => i);
Print("foreach", Map(actions, action => action()));

var empty = new List<int>();
Console.WriteLine($"| Порожній | 0 / 0 / 0 | {Filter(empty, n => true).Count} / {Map(empty, n => n).Count} / {GroupBy(empty, n => n).Count} |");
Console.WriteLine($"| Порожній, максимум | відсутній | {Maximum(empty)?.ToString() ?? "відсутній"} |");
Console.WriteLine($"| Жоден не підходить | 0 | {Filter(new List<int> { 1, 2 }, n => n % 3 == 0).Count} |");
Console.WriteLine($"| Усі в одній групі | 1 | {GroupBy(new List<int> { 1, 2, 3 }, n => 0).Count} |");
Console.WriteLine($"| Один елемент, сума та максимум | 7 / 7 | {Aggregate(new List<int> { 7 }, 0, (s, n) => s + n)} / {Maximum(new List<int> { 7 })} |");

static bool IsVowel(char c) => "аеєиіїоуюя".IndexOf(char.ToLowerInvariant(c)) >= 0;

static int CountVowels(string word)
{
    int count = 0;
    foreach (char c in word)
        if (IsVowel(c))
            count++;
    return count;
}

static int? Maximum(List<int> values) =>
    Aggregate(values, (int?)null, (max, n) => max is null || n > max.Value ? n : max);

static void Print<T>(string title, List<T> values) =>
    Console.WriteLine($"{title} ({values.Count}): {string.Join(", ", values)}");

static void PrintGroups<T, TKey>(string title, Dictionary<TKey, List<T>> groups) where TKey : notnull
{
    Console.WriteLine(title);
    foreach (var group in groups)
        Console.WriteLine($"  {group.Key}: {group.Value.Count}");
}

static class Algorithms
{
    public static List<T> Filter<T>(List<T> source, Func<T, bool> keep)
    {
        var result = new List<T>();
        foreach (T item in source)
            if (keep(item))
                result.Add(item);
        return result;
    }

    public static List<TOut> Map<TIn, TOut>(List<TIn> source, Func<TIn, TOut> convert)
    {
        var result = new List<TOut>(source.Count);
        foreach (TIn item in source)
            result.Add(convert(item));
        return result;
    }

    public static TAcc Aggregate<T, TAcc>(List<T> source, TAcc start, Func<TAcc, T, TAcc> step)
    {
        foreach (T item in source)
            start = step(start, item);
        return start;
    }

    public static Dictionary<TKey, List<T>> GroupBy<T, TKey>(List<T> source, Func<T, TKey> keyOf)
        where TKey : notnull
    {
        var result = new Dictionary<TKey, List<T>>();
        foreach (T item in source)
        {
            TKey key = keyOf(item);
            if (!result.TryGetValue(key, out var group))
            {
                group = new List<T>();
                result.Add(key, group);
            }
            group.Add(item);
        }
        return result;
    }
}

sealed record Student(string Name, string Group, int Score)
{
    public override string ToString() => $"{Name} ({Group}, {Score})";
}

static class Students
{
    public static List<Student> FromLab6() =>
    [
        new Student("Коваленко", "КН-3/1", 88),
        new Student("Мельник",   "КН-3/2", 64),
        new Student("Іщенко",    "КН-3/1", 93),
        new Student("Бондар",    "КН-3/1", 71),
        new Student("Савченко",  "КН-3/2", 55),
        new Student("Ткачук",    "КН-3/2", 90),
        new Student("Кравець",   "КН-3/1", 76),
        new Student("Іванів",    "КН-3/1", 48),
        new Student("Мороз",     "КН-3/2", 82),
        new Student("Шевчук",    "КН-3/1", 67),
        new Student("Бойко",     "КН-3/2", 95),
        new Student("Кушнір",    "КН-3/2", 73),
    ];
}
