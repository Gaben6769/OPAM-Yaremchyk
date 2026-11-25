using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("N = 31, K = 6, a = 1, b = 3, c = 1");
if (args is ["--threshold-only"])
{
    MeasureThreshold();
    return;
}
(string Name, Sort<int> Run)[] sorts =
[
    ("Вставками", Sorts.Insertion),
    ("Швидке", Sorts.Quick),
    ("Бінарні вставки", Sorts.BinaryInsertion)
];

var trace = Generate(7);
Console.WriteLine("| Прохід | Стан масиву |");
Console.WriteLine("|---|---|");
Console.WriteLine($"| Старт | {string.Join(", ", trace)} |");
for (int i = 1; i < trace.Length; i++)
{
    var prefix = trace[..(i + 1)];
    Sorts.Insertion(prefix, Compare, out _, out _);
    prefix.CopyTo(trace, 0);
    Console.WriteLine($"| {i} | {string.Join(", ", trace)} |");
}

foreach (var sort in sorts)
{
    Console.WriteLine($"### {sort.Name}");
    Console.WriteLine("| n | Випадковий | Відсортований | Навпаки | Майже |");
    Console.WriteLine("|---:|---:|---:|---:|---:|");
    foreach (int size in new[] { 100, 1000, 10000 })
    {
        var cells = new List<string>();
        foreach (var input in MakeInputs(size))
        {
            var copy = (int[])input.Clone();
            sort.Run(copy, Compare, out int comparisons, out int moves);
            Checks.Validate(copy, input, Compare);
            cells.Add($"{comparisons} / {moves}");
        }
        Console.WriteLine($"| {size} | {string.Join(" | ", cells)} |");
    }
}
var equal = new int[1000];
Sorts.Quick(equal, Compare, out int equalComparisons, out int equalMoves);
Console.WriteLine($"Швидке, 1000 однакових: {equalComparisons} порівнянь, {equalMoves} записів.");

Console.WriteLine("| Крайовий випадок | Вставками | Швидке | Бінарні вставки |");
Console.WriteLine("|---|---|---|---|");
(string Name, int[] Values)[] edges =
[
    ("Порожній", []), ("Один", [1]), ("Два в порядку", [1, 2]),
    ("Два навпаки", [2, 1]), ("Однакові", [2, 2, 2, 2]),
    ("Половина кожного", [2, 1, 2, 1, 2, 1])
];
foreach (var edge in edges)
{
    foreach (var sort in sorts)
    {
        var copy = (int[])edge.Values.Clone();
        sort.Run(copy, Compare, out _, out _);
        Checks.Validate(copy, edge.Values, Compare);
    }
    Console.WriteLine($"| {edge.Name} | пройдено | пройдено | пройдено |");
}

var students = Students.FromLab6();
var original = new List<Student>(students);
foreach (var sample in students)
    original.Add(sample with { Score = 88 });
var surnames = CultureInfo.GetCultureInfo("uk-UA").CompareInfo;
original.Sort((a, b) => surnames.Compare(a.Name, b.Name, CompareOptions.None));
Console.WriteLine("| Алгоритм | Очікувано | Фактично | Перший контрприклад |");
Console.WriteLine("|---|---|---|---|");
CheckStability("Вставками", "стабільне", Sorts.Insertion<Student>);
CheckStability("Швидке", "нестабільне", Sorts.Quick<Student>);
CheckStability("Бінарні вставки", "стабільне", Sorts.BinaryInsertion<Student>);
CheckStability("Array.Sort", "нестабільне", StandardSort<Student>);
CheckStability("OrderBy", "стабільне", EnumerableSort);

Console.WriteLine("| n | Вставками, мс | Швидке, мс | Бінарні вставки, мс | Array.Sort, мс |");
Console.WriteLine("|---:|---:|---:|---:|---:|");
foreach (int size in new[] { 1000, 10000, 100000, 1000000 })
{
    var input = Generate(size);
    string simpleTime = size < 1000000
        ? Measurements.Measure(input, Compare, Sorts.Insertion<int>).ToString("F3")
        : "—";
    Console.WriteLine($"| {size} | {simpleTime} | {Measurements.Measure(input, Compare, Sorts.Quick<int>):F3} | {Measurements.Measure(input, Compare, Sorts.BinaryInsertion<int>):F3} | {Measurements.Measure(input, Compare, StandardSort<int>):F3} |");
}
MeasureThreshold();

static void MeasureThreshold()
{
    int lower = 0;
    int upper = 1000;
    while (Measurements.Measure(Generate(upper), Compare, Sorts.Insertion<int>) <= 1000)
    {
        lower = upper;
        upper *= 2;
    }
    while (upper - lower > 1000)
    {
        int middle = lower + (upper - lower) / 2;
        double time = Measurements.Measure(Generate(middle), Compare, Sorts.Insertion<int>);
        if (time <= 1000)
            lower = middle;
        else
            upper = middle;
    }
    Console.WriteLine($"Межа 1 с: від {lower} до {upper} елементів за найшвидшим із трьох запусків.");
}

void CheckStability(string name, string expected, Sort<Student> sort)
{
    var sorted = original.ToArray();
    sort(sorted, CompareScore, out _, out _);
    Checks.Validate(sorted, original.ToArray(), CompareScore);
    string first = "—";
    for (int i = 1; i < sorted.Length; i++)
    {
        if (sorted[i - 1].Score == sorted[i].Score &&
            surnames.Compare(sorted[i - 1].Name, sorted[i].Name, CompareOptions.None) > 0)
        {
            first = $"{sorted[i - 1].Name} → {sorted[i].Name} ({sorted[i].Score})";
            break;
        }
    }
    Console.WriteLine($"| {name} | {expected} | {(first == "—" ? "порядок збережено" : "порядок змінено")} | {first} |");
}

static int Compare(int a, int b) => a.CompareTo(b);
static int CompareScore(Student a, Student b) => Compare(a.Score, b.Score);

static int[] Generate(int size)
{
    var random = new Random(31);
    var result = new int[size];
    for (int i = 0; i < size; i++)
        result[i] = random.Next(121);
    return result;
}

static int[][] MakeInputs(int size)
{
    var input = Generate(size);
    var sorted = (int[])input.Clone();
    Array.Sort(sorted);
    var reversed = (int[])sorted.Clone();
    for (int i = 0; i < size / 2; i++)
        (reversed[i], reversed[size - 1 - i]) = (reversed[size - 1 - i], reversed[i]);
    var almost = (int[])sorted.Clone();
    var random = new Random(31);
    for (int i = 0; i < 6; i++)
    {
        int left = random.Next(size), right = random.Next(size);
        (almost[left], almost[right]) = (almost[right], almost[left]);
    }
    return [input, sorted, reversed, almost];
}

static void StandardSort<T>(T[] array, Func<T, T, int> cmp, out int comparisons, out int moves)
{
    Array.Sort(array, (a, b) => cmp(a, b));
    comparisons = moves = 0;
}

static void EnumerableSort(Student[] array, Func<Student, Student, int> cmp,
    out int comparisons, out int moves)
{
    var stable = array.OrderBy(s => s, Comparer<Student>.Create((a, b) => cmp(a, b))).ToArray();
    stable.CopyTo(array, 0);
    comparisons = moves = 0;
}

