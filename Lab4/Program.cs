using System;
using System.Collections.Generic;
using System.Diagnostics;

class Program
{
    const int N = 31;
    const int K = 33;

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine($"=== Лабораторна робота 4 (Варіант: N = {N}, K = {K}) ===\n");

        // --- Генерація основного списку ---
        List<int> mainList = GenerateMainList(20, N);
        Console.Write("Основний список (20 елементів): ");
        PrintList(mainList);
        Console.WriteLine();

        // --- Завдання 1 ---
        Console.WriteLine("--- Завдання 1: Побудова нового списку за критерієм (кратні 3) ---");
        List<int> task1Result = FilterMultiplesOfThree(mainList);
        Console.Write("Результат: ");
        PrintList(task1Result);
        Console.WriteLine($"Кількість елементів (Count): {task1Result.Count}\n");

        // --- Завдання 2 ---
        Console.WriteLine("--- Завдання 2: Вилучення елементів, кратних 3 (три способи) ---");
        
        // Спроба 1: Хибна (цикл від початку з RemoveAt)
        List<int> copy1 = new List<int>(mainList);
        RemoveMultiplesWrong(copy1);
        Console.Write("1. Хибний спосіб (RemoveAt(i) з початку): ");
        PrintList(copy1);

        // Спроба 2: Цикл з кінця
        List<int> copy2 = new List<int>(mainList);
        RemoveMultiplesFromEnd(copy2);
        Console.Write("2. Правильний спосіб (з кінця):           ");
        PrintList(copy2);

        // Спроба 3: Новий список
        List<int> copy3 = new List<int>(mainList);
        List<int> task2NewList = RemoveMultiplesNewList(copy3);
        Console.Write("3. Правильний спосіб (новий список):      ");
        PrintList(task2NewList);

        // Перевірка рівності варіантів 2 та 3
        bool task2Equals = AreListsEqual(copy2, task2NewList);
        Console.WriteLine($"Результати способів 2 і 3 однакові: {task2Equals}\n");

        // --- Завдання 3 ---
        Console.WriteLine("--- Завдання 3: Злиття двох упорядкованих списків ---");
        int len1 = 10 + K % 5; // 13
        int len2 = 6 + K % 4;  // 7
        List<int> list1 = GenerateSortedList(len1, N);
        List<int> list2 = GenerateSortedList(len2, N + 100);

        Console.Write("Список 1 (13 ел.): ");
        PrintList(list1);
        Console.Write("Список 2 (7 ел.):  ");
        PrintList(list2);

        List<int> mergedList = MergeSortedLists(list1, list2);
        Console.Write("Злитий список:     ");
        PrintList(mergedList);

        bool isSorted = CheckIsSorted(mergedList);
        Console.WriteLine($"Результат впорядковано за зростанням: {isSorted}\n");

        // --- Завдання 4 ---
        Console.WriteLine("--- Завдання 4: Розділення за індексами (парні та непарні) ---");
        SplitByIndices(mainList, out List<int> evenIndexList, out List<int> oddIndexList);

        Console.Write("Елементи на парних індексах:   ");
        PrintList(evenIndexList);
        Console.Write("Елементи на непарних індексах: ");
        PrintList(oddIndexList);

        bool countCheck = (evenIndexList.Count + oddIndexList.Count) == mainList.Count;
        Console.WriteLine($"Сума Count обох списків ({evenIndexList.Count} + {oddIndexList.Count}) дорівнює початковому ({mainList.Count}): {countCheck}\n");

        // --- Завдання 5 ---
        Console.WriteLine("--- Завдання 5: Замір часу (Benchmark) ---");
        RunBenchmark();
    }

    // --- Допоміжні функції ---
    static List<int> GenerateMainList(int count, int seed)
    {
        Random rnd = new Random(seed);
        List<int> list = new List<int>(count);
        for (int i = 0; i < count; i++)
        {
            list.Add(rnd.Next(-9, 10));
        }
        return list;
    }

    static void PrintList(List<int> list)
    {
        Console.WriteLine("[" + string.Join(", ", list) + "]");
    }

    static bool AreListsEqual(List<int> a, List<int> b)
    {
        if (a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    // --- Завдання 1 ---
    static List<int> FilterMultiplesOfThree(List<int> input)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < input.Count; i++)
        {
            if (input[i] % 3 == 0)
            {
                result.Add(input[i]);
            }
        }
        return result;
    }

    // --- Завдання 2 ---
    // 1. Хибний спосіб
    static void RemoveMultiplesWrong(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] % 3 == 0)
            {
                list.RemoveAt(i);
            }
        }
    }

    // 2. Правильний спосіб з кінця
    static void RemoveMultiplesFromEnd(List<int> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] % 3 == 0)
            {
                list.RemoveAt(i);
            }
        }
    }

    // 3. Правильний спосіб через новий список
    static List<int> RemoveMultiplesNewList(List<int> input)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < input.Count; i++)
        {
            if (input[i] % 3 != 0)
            {
                result.Add(input[i]);
            }
        }
        return result;
    }

    // --- Завдання 3 ---
    static List<int> GenerateSortedList(int count, int seed)
    {
        Random rnd = new Random(seed);
        List<int> list = new List<int>(count);
        int current = rnd.Next(1, 10);
        list.Add(current);

        for (int i = 1; i < count; i++)
        {
            current += rnd.Next(1, 10);
            list.Add(current);
        }
        return list;
    }

    static List<int> MergeSortedLists(List<int> a, List<int> b)
    {
        List<int> result = new List<int>(a.Count + b.Count);
        int i = 0, j = 0;

        while (i < a.Count && j < b.Count)
        {
            if (a[i] <= b[j])
            {
                result.Add(a[i]);
                i++;
            }
            else
            {
                result.Add(b[j]);
                j++;
            }
        }

        while (i < a.Count)
        {
            result.Add(a[i]);
            i++;
        }

        while (j < b.Count)
        {
            result.Add(b[j]);
            j++;
        }

        return result;
    }

    static bool CheckIsSorted(List<int> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            if (list[i] > list[i + 1])
                return false;
        }
        return true;
    }

    // --- Завдання 4 ---
    static void SplitByIndices(List<int> input, out List<int> evenList, out List<int> oddList)
    {
        evenList = new List<int>();
        oddList = new List<int>();

        for (int i = 0; i < input.Count; i++)
        {
            if (i % 2 == 0)
            {
                evenList.Add(input[i]);
            }
            else
            {
                oddList.Add(input[i]);
            }
        }
    }

    // --- Завдання 5: Заміри часу ---
    static void RunBenchmark()
    {
        // Прогрів JIT
        List<int> warmup = new List<int>();
        for (int i = 0; i < 1000; i++) warmup.Add(i);

        Stopwatch sw = new Stopwatch();

        // 1. Add без місткості
        sw.Restart();
        List<int> list1 = new List<int>();
        for (int i = 0; i < 100000; i++) list1.Add(i);
        sw.Stop();
        long t1 = sw.ElapsedMilliseconds;

        // 2. Add з місткістю
        sw.Restart();
        List<int> list2 = new List<int>(100000);
        for (int i = 0; i < 100000; i++) list2.Add(i);
        sw.Stop();
        long t2 = sw.ElapsedMilliseconds;

        // 3. Insert(0, x)
        sw.Restart();
        List<int> list3 = new List<int>();
        for (int i = 0; i < 100000; i++) list3.Insert(0, i);
        sw.Stop();
        long t3 = sw.ElapsedMilliseconds;

        // 4. Contains 1000 разів (шукаємо елемент, якого немає)
        sw.Restart();
        for (int i = 0; i < 1000; i++)
        {
            bool found = CustomContains(list1, -999999);
        }
        sw.Stop();
        long t4 = sw.ElapsedMilliseconds;

        Console.WriteLine($"100 000 x Add(x) (без місткості): {t1} мс");
        Console.WriteLine($"100 000 x Add(x) (з місткістю):   {t2} мс");
        Console.WriteLine($"100 000 x Insert(0, x):          {t3} мс");
        Console.WriteLine($"1 000 x Contains(-999999):       {t4} мс");
    }

    // Реалізація пошуку без LINQ
    static bool CustomContains(List<int> list, int value)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == value) return true;
        }
        return false;
    }
}