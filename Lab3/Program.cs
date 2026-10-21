using System;
using System.IO;

class Program
{
    // Замініть значення N та ім'я під свій варіант!
    const int N = 7;
    const string NAME = "Олексій";
    static readonly int K = GetLetterNumber(NAME[0]);

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine($"=== Варіант: N = {N}, Ім'я = {NAME}, K = {K} ===");
        int a = N % 3;
        int b = N % 4;
        int c = N % 5;
        Console.WriteLine($"a = {a}, b = {b}, c = {c}\n");

        // --- Завдання 1 ---
        Console.WriteLine("--- Завдання 1: Вкладена структура (Обхід директорії) ---");
        Console.Write("Введіть шлях до папки: ");
        string dirPath = Console.ReadLine();

        if (!Directory.Exists(dirPath))
        {
            Console.WriteLine("Помилка: вказана папка не існує!");
        }
        else
        {
            var (csCount, csTotalSize) = Task1_CountCsFiles(new DirectoryInfo(dirPath));
            Console.WriteLine($"Знайдено .cs файлів: {csCount}, Сумарний розмір: {csTotalSize} байт");
        }
        Console.WriteLine();

        // --- Завдання 2 ---
        Console.WriteLine("--- Завдання 2: Розбиття навпіл ---");
        Random rnd = new Random(N);
        int[] arr16 = new int[16];
        for (int i = 0; i < 16; i++)
            arr16[i] = rnd.Next(-20, 21);

        Console.WriteLine("Масив: " + string.Join(", ", arr16));
        int targetValue = K % 10;
        int maxDepth = 0;

        int count = Task2_CountGreaterThanK(arr16, 0, arr16.Length - 1, 1, ref maxDepth, targetValue);
        Console.WriteLine($"Кількість елементів, більших за {targetValue}: {count}");
        Console.WriteLine($"Максимальна глибина рекурсії: {maxDepth} (очікувана log2(16) + 1 = 5)");
        Console.WriteLine();

        // --- Завдання 3 ---
        Console.WriteLine("--- Завдання 3: Перебір із поверненням (Сходинки) ---");
        int M = K % 8 + 5; // Кількість сходинок
        Console.WriteLine($"Усі шляхи підйому на {M} сходинок (кроками по 1 або 2):");
        int totalWays = Task3_StairsBacktracking(M, "");
        Console.WriteLine($"Усього варіантів: {totalWays}\n");

        // --- Завдання 4 ---
        Console.WriteLine("--- Завдання 4: Трасування (Масив з 4 елементів) ---");
        int[] arr4 = new int[4];
        rnd = new Random(N);
        for (int i = 0; i < 4; i++) arr4[i] = rnd.Next(-20, 21);

        Task4_TraceCountGreaterThanK(arr4, 0, arr4.Length - 1, 0, targetValue);
        Console.WriteLine();

        // --- Завдання 5 ---
        Console.WriteLine("--- Завдання 5: Ітеративна версія (Цикл) ---");
        int countIterative = Task5_CountGreaterThanK_Iterative(arr16, targetValue);
        Console.WriteLine($"Результат циклу: {countIterative}");
        Console.WriteLine($"Результати збігаються: {count == countIterative}");
    }

    // --- Допоміжна функція визначення номера літери ---
    static int GetLetterNumber(char c)
    {
        string alphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ";
        int index = alphabet.IndexOf(char.ToUpper(c));
        return index != -1 ? index + 1 : 1;
    }

    // --- Завдання 1: Кортеж (кількість, розмір) ---
    static (int count, long totalSize) Task1_CountCsFiles(DirectoryInfo dir)
    {
        int count = 0;
        long totalSize = 0;

        try
        {
            foreach (FileInfo file in dir.GetFiles())
            {
                if (file.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                    totalSize += file.Length;
                }
            }

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                var (subCount, subSize) = Task1_CountCsFiles(subDir);
                count += subCount;
                totalSize += subSize;
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Пропускаємо системні/захищені папки
        }

        return (count, totalSize);
    }

    // --- Завдання 2: Розбиття навпіл ---
    static int Task2_CountGreaterThanK(int[] a, int lo, int hi, int currentDepth, ref int maxDepth, int K_mod10)
    {
        if (currentDepth > maxDepth)
            maxDepth = currentDepth;

        // Базовий випадок: один елемент
        if (lo == hi)
            return a[lo] > K_mod10 ? 1 : 0;

        int mid = lo + (hi - lo) / 2;
        int leftResult = Task2_CountGreaterThanK(a, lo, mid, currentDepth + 1, ref maxDepth, K_mod10);
        int rightResult = Task2_CountGreaterThanK(a, mid + 1, hi, currentDepth + 1, ref maxDepth, K_mod10);

        return leftResult + rightResult;
    }

    // --- Завдання 3: Backtracking (Сходинки) ---
    static int Task3_StairsBacktracking(int remainingSteps, string path)
    {
        if (remainingSteps == 0)
        {
            Console.WriteLine(path.TrimEnd('-', ' '));
            return 1;
        }

        int count = 0;
        // Крок 1
        if (remainingSteps >= 1)
            count += Task3_StairsBacktracking(remainingSteps - 1, path + "1 -> ");

        // Крок 2
        if (remainingSteps >= 2)
            count += Task3_StairsBacktracking(remainingSteps - 2, path + "2 -> ");

        return count;
    }

    // --- Завдання 4: Трасування ---
    static int Task4_TraceCountGreaterThanK(int[] a, int lo, int hi, int depth, int K_mod10)
    {
        string indent = new string(' ', depth * 2);
        Console.WriteLine($"{indent}enter Count({lo}..{hi})");

        int result;
        if (lo == hi)
        {
            result = a[lo] > K_mod10 ? 1 : 0;
            Console.WriteLine($"{indent}  leaf({lo}) = {a[lo]} -> {result}");
        }
        else
        {
            int mid = lo + (hi - lo) / 2;
            int left = Task4_TraceCountGreaterThanK(a, lo, mid, depth + 1, K_mod10);
            int right = Task4_TraceCountGreaterThanK(a, mid + 1, hi, depth + 1, K_mod10);
            result = left + right;
        }

        Console.WriteLine($"{indent}exit  Count({lo}..{hi}) -> {result}");
        return result;
    }

    // --- Завдання 5: Циклічна версія ---
    static int Task5_CountGreaterThanK_Iterative(int[] a, int K_mod10)
    {
        int count = 0;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] > K_mod10)
                count++;
        }
        return count;
    }
}