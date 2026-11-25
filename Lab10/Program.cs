using System.Globalization;
using System.Text;
using static TextAlgorithms;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("N = 31, K = 6, a = 1, b = 3, c = 1");
var prose = File.ReadAllText(Path.Join(AppContext.BaseDirectory, "prose.txt"))[..50000];

Console.WriteLine("| Додавань | +=, мс | StringBuilder, мс |");
Console.WriteLine("|---:|---:|---:|");
foreach (int size in new[] { 10000, 100000 })
    Console.WriteLine($"| {size} | {Measurements.Run(() => Concat(size)):F3} | {Measurements.Run(() => Build(size)):F3} |");

var words = Words(prose);
(string A, string B)[] pairs = new (string, string)[100000];
for (int i = 0; i < pairs.Length; i++)
{
    string left = words[i % words.Length];
    string right = i % 2 == 0 ? left : words[(i + 1) % words.Length];
    pairs[i] = (left, right.ToUpperInvariant());
}
Console.WriteLine($"Порівняння регістру, 100 000 пар: ToLower {Measurements.Run(LowercaseEqual):F3} мс, OrdinalIgnoreCase {Measurements.Run(OrdinalEqual):F3} мс.");
if (LowercaseEqual() != OrdinalEqual())
    throw new InvalidOperationException("Порівняння мають дати однакову кількість збігів.");

string[] alphabetWords =
[
    "ґанок", "ґава", "ґазда", "ґатунок", "ґвалт", "ґедзь", "ґелґіт", "ґердан",
    "ґирлиґа", "ґлей", "ґніт", "ґонт", "ґрати", "ґречний", "ґринджоли", "ґроно",
    "ґрунт", "ґудзик", "ґума", "ґуральня", "гадка", "гострий", "давній", "жито"
];
var ordinal = (string[])alphabetWords.Clone();
Array.Sort(ordinal, StringComparer.Ordinal);
var cultural = (string[])alphabetWords.Clone();
Array.Sort(cultural, StringComparer.Create(CultureInfo.GetCultureInfo("uk-UA"), false));
Console.WriteLine($"Ординальний порядок: {string.Join(", ", ordinal)}.");
Console.WriteLine($"Порядок uk-UA: {string.Join(", ", cultural)}.");

const int limit = 11;
Console.WriteLine("| Символів | Вкладені цикли, мс | Словник, мс | Відношення |");
Console.WriteLine("|---:|---:|---:|---:|");
foreach (int size in new[] { 1000, 10000, 50000 })
{
    string sample = prose[..size];
    CheckTop(MostFrequentSlow(sample, limit), MostFrequentFast(sample, limit));
    double slow = Measurements.Run(() => MostFrequentSlow(sample, limit));
    double fast = Measurements.Run(() => MostFrequentFast(sample, limit));
    Console.WriteLine($"| {size} | {slow:F3} | {fast:F3} | {slow / fast:F2} |");
}
Console.WriteLine($"Слів у вибірці: {words.Length}. Найчастіші: {string.Join(", ", MostFrequentFast(prose, limit))}.");

string pattern = new string('Д', 959) + 'Е';
string word = "громада";
int match = prose.IndexOf(word, StringComparison.Ordinal);
if (match <= 0)
    throw new InvalidOperationException("Потрібне слово має бути всередині прози.");
Console.WriteLine("| Вхід | Шаблон | Індекс | Порівнянь | Власний, мс | string.IndexOf, мс |");
Console.WriteLine("|---|---|---:|---:|---:|---:|");
Search("Проза", prose, word, word);
Search("Проза", prose, "ґудзикозавр", "ґудзикозавр");
Search("Повтори", new string('Д', 10000), pattern, "959 × Д, потім Е");

const int rod = 36;
var random = new Random(31);
int optimal = 0, largestGap = 0, fewestPieces = int.MaxValue;
double worstPercent = 0;
string smallest = "";
for (int test = 0; test < 100; test++)
{
    var pieces = GeneratePieces(random.Next(1, 16));
    int greedy = RodCutting.Greedy(rod, pieces);
    int exact = RodCutting.Exhaustive(rod, pieces);
    if (exact > greedy)
        throw new InvalidOperationException("Перебір пропустив допустиму комбінацію.");
    if (greedy == exact)
    {
        optimal++;
        continue;
    }
    int gap = greedy - exact;
    largestGap = Math.Max(largestGap, gap);
    worstPercent = Math.Max(worstPercent, 100d * gap / rod);
    if (pieces.Length < fewestPieces)
    {
        fewestPieces = pieces.Length;
        smallest = $"[{string.Join(", ", pieces)}], відходи {greedy} / {exact} см";
    }
}
Console.WriteLine("| Показник | Значення |");
Console.WriteLine("|---|---|");
Console.WriteLine($"| Оптимальних жадібних розв'язків | {optimal}/100 |");
Console.WriteLine($"| Найбільша додаткова частка відходу | {worstPercent:F2}% від довжини стрижня ({largestGap} см) |");
Console.WriteLine($"| Найменший за кількістю деталей контрприклад у 100 тестах | {smallest} |");
foreach (int size in new[] { 15, 20 })
{
    var sample = GeneratePieces(size);
    Console.WriteLine($"| Перебір, {size} деталей | {Measurements.Run(() => RodCutting.Exhaustive(rod, sample)):F3} мс |");
}

CheckLetters(LetterArray(prose), LetterDictionary(prose));
double arrayTime = Measurements.Run(() => LetterArray(prose));
double dictionaryTime = Measurements.Run(() => LetterDictionary(prose));
Console.WriteLine($"33 літери: масив {arrayTime:F3} мс, словник {dictionaryTime:F3} мс, відношення {dictionaryTime / arrayTime:F2}.");

Console.WriteLine("| Крайовий випадок | Очікувано | Отримано |");
Console.WriteLine("|---|---|---|");
CheckWords("Порожній текст", "", "");
CheckWords("Одна літера", "Д", "(д, 1)");
CheckWords("Повтори", "ДДДД", "(дддд, 1)");
CheckWords("Зайві пробіли", "  А  А  б  ", "(а, 2), (б, 1)");
CheckEdge("Порожній текст, непорожній шаблон", -1, IndexOf("", "Д", out _));
CheckEdge("Шаблон довший за текст", -1, IndexOf("Д", "ДЕ", out _));
CheckEdge("Порожній шаблон", 0, IndexOf("Д", "", out _));
CheckEdge("Стрижень без деталей", 36, RodCutting.Exhaustive(rod, []));
CheckEdge("Стрижень з однією деталлю", 1, RodCutting.Exhaustive(rod, [35]));
CheckEdge("Жадібно без деталей", 36, RodCutting.Greedy(rod, []));
CheckEdge("Жадібно з однією деталлю", 1, RodCutting.Greedy(rod, [35]));

static string Concat(int length)
{
    string result = "";
    for (int i = 0; i < length; i++)
        result += 'Д';
    return result;
}

static string Build(int length)
{
    var result = new StringBuilder();
    for (int i = 0; i < length; i++)
        result.Append('Д');
    return result.ToString();
}

int LowercaseEqual()
{
    int count = 0;
    foreach (var pair in pairs)
        if (pair.A.ToLower() == pair.B.ToLower())
            count++;
    return count;
}

int OrdinalEqual()
{
    int count = 0;
    foreach (var pair in pairs)
        if (string.Equals(pair.A, pair.B, StringComparison.OrdinalIgnoreCase))
            count++;
    return count;
}

static void Search(string sample, string text, string pattern, string label)
{
    int index = IndexOf(text, pattern, out int count);
    if (index != text.IndexOf(pattern, StringComparison.Ordinal))
        throw new InvalidOperationException("Пошук не узгоджується з еталоном.");
    double own = Measurements.Run(() => IndexOf(text, pattern, out _));
    double reference = Measurements.Run(() => text.IndexOf(pattern, StringComparison.Ordinal));
    Console.WriteLine($"| {sample} | {label} | {index} | {count} | {own:F3} | {reference:F3} |");
}

int[] GeneratePieces(int size)
{
    var pieces = new int[size];
    for (int i = 0; i < size; i++)
        pieces[i] = random.Next(1, rod + 1);
    return pieces;
}

static void CheckWords(string label, string source, string expected)
{
    var fast = MostFrequentFast(source, 11);
    CheckTop(MostFrequentSlow(source, 11), fast);
    string actual = string.Join(", ", fast);
    if (actual != expected)
        throw new InvalidOperationException(label);
    Console.WriteLine($"| {label} | {expected} | {actual} |");
}

static void CheckEdge(string label, int expected, int actual)
{
    if (expected != actual)
        throw new InvalidOperationException(label);
    Console.WriteLine($"| {label} | {expected} | {actual} |");
}
