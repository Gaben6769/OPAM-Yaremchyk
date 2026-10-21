using System;
using System.Collections.Generic;
using System.Diagnostics;

class Student
{
    public string Name;
    public string Group;
    public int Score;

    public Student(string name, string group, int score)
    {
        Name = name;
        Group = group;
        Score = score;
    }
}

class Program
{
    const int N = 31;
    const int K = 33;

    static readonly string SampleText =
        "Алгоритм описує послідовність дій. Алгоритм можна записати словами, " +
        "а можна записати кодом. Кодом алгоритм стає тоді, коли його " +
        "виконує машина, а не людина. Одна задача — багато алгоритмів.";

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine($"=== Лабораторна робота 6 (Варіант: N = {N}, K = {K}) ===\n");

        // --- Завдання 1 ---
        Console.WriteLine("--- Завдання 1: Частоти літер у тексті (без регістру) ---");
        int threshold = K % 3 + 2; // 2
        var letterFreq = Task1_CountLetterFrequencies(SampleText);

        Console.WriteLine($"Загальна кількість різних літер: {letterFreq.Count}");
        Console.WriteLine($"Літери з частотою >= {threshold}:");
        foreach (var kvp in letterFreq)
        {
            if (kvp.Value >= threshold)
            {
                Console.WriteLine($"  '{kvp.Key}': {kvp.Value}");
            }
        }
        Console.WriteLine();

        // --- Завдання 2 ---
        Console.WriteLine("--- Завдання 2: Групування студентів за кількістю літер у прізвищі ---");
        List<Student> students = GetInitialStudents();
        var groupedStudents = Task2_GroupStudentsBySurnameLength(students);

        foreach (var kvp in groupedStudents)
        {
            int totalScore = 0;
            for (int i = 0; i < kvp.Value.Count; i++)
            {
                totalScore += kvp.Value[i].Score;
            }
            double avgScore = kvp.Value.Count > 0 ? (double)totalScore / kvp.Value.Count : 0;

            Console.WriteLine($"Ключ (довжина прізвища = {kvp.Key}): кількість = {kvp.Value.Count}, середня оцінка = {avgScore:F2}");
            for (int i = 0; i < kvp.Value.Count; i++)
            {
                Console.WriteLine($"   - {kvp.Value[i].Name} ({kvp.Value[i].Group}): {kvp.Value[i].Score}");
            }
        }
        Console.WriteLine();

        // --- Завдання 3 ---
        Console.WriteLine("--- Завдання 3: Вилучення дублікатів двома способами ---");
        int rangeMax = K % 10 + 6; // 9
        List<int> origList = GenerateRandomList(25, 1, rangeMax, N);
        Console.WriteLine("Оригінальний список (25 елементів): " + FormatList(origList));

        // 1. Без збереження порядку
        HashSet<int> unorderedSet = new HashSet<int>(origList);
        List<int> unorderedList = new List<int>(unorderedSet);
        Console.WriteLine("1. Без збереження порядку (HashSet): " + FormatList(unorderedList));

        // 2. Зі збереженням порядку
        List<int> orderedList = Task3_RemoveDuplicatesKeepOrder(origList);
        Console.WriteLine("2. Зі збереженням порядку:          " + FormatList(orderedList));
        Console.WriteLine();

        // --- Завдання 4 ---
        Console.WriteLine("--- Завдання 4: Набори (Елементи, що є в 1-му і відсутні в 2-му) ---");
        int setMax = 20 + K; // 53
        List<int> set1List = GenerateRandomList(30, 1, setMax, N);
        List<int> set2List = GenerateRandomList(30, 1, setMax, N + 100);

        Console.WriteLine("Набір 1: " + FormatList(set1List));
        Console.WriteLine("Набір 2: " + FormatList(set2List));

        // Спосіб 1: HashSet
        List<int> diffHashSet = Task4_DifferenceHashSet(set1List, set2List);
        // Спосіб 2: Вкладені цикли
        List<int> diffLoops = Task4_DifferenceLoops(set1List, set2List);

        Console.WriteLine("Різниця (через HashSet): " + FormatList(diffHashSet));
        Console.WriteLine("Різниця (через цикли):   " + FormatList(diffLoops));

        bool task4Matches = AreListsEqualIgnoreOrder(diffHashSet, diffLoops);
        Console.WriteLine($"Результати обох способів збігаються: {task4Matches}\n");

        // --- Завдання 5 ---
        Console.WriteLine("--- Завдання 5: Замір виграшу (Benchmark) ---");
        RunBenchmark();
    }

    // --- Завдання 1 ---
    static Dictionary<char, int> Task1_CountLetterFrequencies(string text)
    {
        Dictionary<char, int> dict = new Dictionary<char, int>();
        for (int i = 0; i < text.Length; i++)
        {
            char c = char.ToLower(text[i]);
            if (char.IsLetter(c))
            {
                if (dict.TryGetValue(c, out int currentCount))
                {
                    dict[c] = currentCount + 1;
                }
                else
                {
                    dict[c] = 1;
                }
            }
        }
        return dict;
    }

    // --- Завдання 2 ---
    static List<Student> GetInitialStudents()
    {
        return new List<Student>
        {
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
            new Student("Кушнір",    "КН-3/2", 73)
        };
    }

    static Dictionary<int, List<Student>> Task2_GroupStudentsBySurnameLength(List<Student> list)
    {
        var dict = new Dictionary<int, List<Student>>();
        for (int i = 0; i < list.Count; i++)
        {
            Student s = list[i];
            int key = s.Name.Length;

            if (!dict.TryGetValue(key, out List<Student> groupList))
            {
                groupList = new List<Student>();
                dict[key] = groupList;
            }
            groupList.Add(s);
        }
        return dict;
    }

    // --- Завдання 3 ---
    static List<int> Task3_RemoveDuplicatesKeepOrder(List<int> input)
    {
        HashSet<int> seen = new HashSet<int>();
        List<int> result = new List<int>();

        for (int i = 0; i < input.Count; i++)
        {
            if (seen.Add(input[i]))
            {
                result.Add(input[i]);
            }
        }
        return result;
    }

    // --- Завдання 4 ---
    static List<int> Task4_DifferenceHashSet(List<int> list1, List<int> list2)
    {
        HashSet<int> set2 = new HashSet<int>(list2);
        HashSet<int> resultSet = new HashSet<int>();

        for (int i = 0; i < list1.Count; i++)
        {
            int val = list1[i];
            if (!set2.Contains(val))
            {
                resultSet.Add(val);
            }
        }
        return new List<int>(resultSet);
    }

    static List<int> Task4_DifferenceLoops(List<int> list1, List<int> list2)
    {
        List<int> result = new List<int>();

        for (int i = 0; i < list1.Count; i++)
        {
            int val = list1[i];

            // Перевіряємо чи є у list2
            bool foundInSet2 = false;
            for (int j = 0; j < list2.Count; j++)
            {
                if (list2[j] == val)
                {
                    foundInSet2 = true;
                    break;
                }
            }

            if (!foundInSet2)
            {
                // Перевіряємо чи вже додали в result (щоб уникнути дублікатів)
                bool alreadyInResult = false;
                for (int k = 0; k < result.Count; k++)
                {
                    if (result[k] == val)
                    {
                        alreadyInResult = true;
                        break;
                    }
                }

                if (!alreadyInResult)
                {
                    result.Add(val);
                }
            }
        }
        return result;
    }

    // --- Завдання 5: Заміри часу ---
    static void RunBenchmark()
    {
        // 1. 10 000 перевірок у 100 000 елементів
        List<int> benchList = GenerateRandomList(100000, 1, 1000000, 42);
        HashSet<int> benchSet = new HashSet<int>(benchList);

        // Прогрів JIT
        for (int i = 0; i < 1000; i++) { bool _ = benchList.Contains(-1); _ = benchSet.Contains(-1); }

        Stopwatch sw = new Stopwatch();

        sw.Restart();
        for (int i = 0; i < 10000; i++)
        {
            bool _ = ListContainsCustom(benchList, -1);
        }
        sw.Stop();
        long t1_list = sw.ElapsedMilliseconds;

        sw.Restart();
        for (int i = 0; i < 10000; i++)
        {
            bool _ = benchSet.Contains(-1);
        }
        sw.Stop();
        long t1_set = sw.ElapsedMilliseconds;

        // 2. Підрахунок частот 50 000 слів
        List<string> words = GenerateRandomWords(50000, 123);

        sw.Restart();
        ListFreqCount(words);
        sw.Stop();
        long t2_list = sw.ElapsedMilliseconds;

        sw.Restart();
        DictFreqCount(words);
        sw.Stop();
        long t2_dict = sw.ElapsedMilliseconds;

        // 3. Вилучення дублікатів з 20 000 чисел
        List<int> dupNumbers = GenerateRandomList(20000, 1, 5000, 99);

        sw.Restart();
        RemoveDuplicatesListCustom(dupNumbers);
        sw.Stop();
        long t3_list = sw.ElapsedMilliseconds;

        sw.Restart();
        RemoveDuplicatesSetCustom(dupNumbers);
        sw.Stop();
        long t3_set = sw.ElapsedMilliseconds;

        // 4. Спільні елементи двох наборів по 5 000 елементів
        List<int> s1 = GenerateRandomList(5000, 1, 20000, 11);
        List<int> s2 = GenerateRandomList(5000, 1, 20000, 22);

        sw.Restart();
        IntersectListsCustom(s1, s2);
        sw.Stop();
        long t4_list = sw.ElapsedMilliseconds;

        sw.Restart();
        IntersectSetsCustom(s1, s2);
        sw.Stop();
        long t4_set = sw.ElapsedMilliseconds;

        Console.WriteLine($"1. 10 000 перевірок (100k елементів): Список = {t1_list} мс, Множина = {t1_set} мс (Виграш: {(t1_set > 0 ? (double)t1_list / t1_set : t1_list):F1}x)");
        Console.WriteLine($"2. Підрахунок частот (50k слів):       Список = {t2_list} мс, Словник = {t2_dict} мс (Виграш: {(t2_dict > 0 ? (double)t2_list / t2_dict : t2_list):F1}x)");
        Console.WriteLine($"3. Вилучення дублікатів (20k чисел):   Список = {t3_list} мс, Множина = {t3_set} мс (Виграш: {(t3_set > 0 ? (double)t3_list / t3_set : t3_list):F1}x)");
        Console.WriteLine($"4. Спільні елементи (5k x 5k):         Список = {t4_list} мс, Множина = {t4_set} мс (Виграш: {(t4_set > 0 ? (double)t4_list / t4_set : t4_list):F1}x)");
    }

    // --- Допоміжні методи для бенчмарку ---
    static bool ListContainsCustom(List<int> list, int val)
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i] == val) return true;
        return false;
    }

    static void ListFreqCount(List<string> words)
    {
        List<string> keys = new List<string>();
        List<int> values = new List<int>();

        for (int i = 0; i < words.Count; i++)
        {
            string w = words[i];
            int idx = -1;
            for (int j = 0; j < keys.Count; j++)
            {
                if (keys[j] == w) { idx = j; break; }
            }

            if (idx != -1)
            {
                values[idx]++;
            }
            else
            {
                keys.Add(w);
                values.Add(1);
            }
        }
    }

    static void DictFreqCount(List<string> words)
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();
        for (int i = 0; i < words.Count; i++)
        {
            string w = words[i];
            if (dict.TryGetValue(w, out int c)) dict[w] = c + 1;
            else dict[w] = 1;
        }
    }

    static List<int> RemoveDuplicatesListCustom(List<int> input)
    {
        List<int> res = new List<int>();
        for (int i = 0; i < input.Count; i++)
        {
            bool exists = false;
            for (int j = 0; j < res.Count; j++)
            {
                if (res[j] == input[i]) { exists = true; break; }
            }
            if (!exists) res.Add(input[i]);
        }
        return res;
    }

    static List<int> RemoveDuplicatesSetCustom(List<int> input)
    {
        HashSet<int> set = new HashSet<int>();
        List<int> res = new List<int>();
        for (int i = 0; i < input.Count; i++)
        {
            if (set.Add(input[i])) res.Add(input[i]);
        }
        return res;
    }

    static List<int> IntersectListsCustom(List<int> a, List<int> b)
    {
        List<int> res = new List<int>();
        for (int i = 0; i < a.Count; i++)
        {
            int val = a[i];
            bool inB = false;
            for (int j = 0; j < b.Count; j++)
            {
                if (b[j] == val) { inB = true; break; }
            }

            if (inB)
            {
                bool inRes = false;
                for (int k = 0; k < res.Count; k++)
                {
                    if (res[k] == val) { inRes = true; break; }
                }
                if (!inRes) res.Add(val);
            }
        }
        return res;
    }

    static List<int> IntersectSetsCustom(List<int> a, List<int> b)
    {
        HashSet<int> setB = new HashSet<int>(b);
        HashSet<int> resSet = new HashSet<int>();
        for (int i = 0; i < a.Count; i++)
        {
            if (setB.Contains(a[i])) resSet.Add(a[i]);
        }
        return new List<int>(resSet);
    }

    // --- Загальні допоміжні методи ---
    static List<int> GenerateRandomList(int count, int min, int max, int seed)
    {
        Random rnd = new Random(seed);
        List<int> list = new List<int>(count);
        for (int i = 0; i < count; i++) list.Add(rnd.Next(min, max + 1));
        return list;
    }

    static List<string> GenerateRandomWords(int count, int seed)
    {
        Random rnd = new Random(seed);
        List<string> list = new List<string>(count);
        string[] samplePool = new string[] { "apple", "banana", "cherry", "date", "elderberry", "fig", "grape", "honeydew" };
        for (int i = 0; i < count; i++) list.Add(samplePool[rnd.Next(samplePool.Length)]);
        return list;
    }

    static string FormatList<T>(List<T> list)
    {
        return "[" + string.Join(", ", list) + "]";
    }

    static bool AreListsEqualIgnoreOrder(List<int> a, List<int> b)
    {
        if (a.Count != b.Count) return false;
        HashSet<int> set = new HashSet<int>(a);
        for (int i = 0; i < b.Count; i++)
        {
            if (!set.Contains(b[i])) return false;
        }
        return true;
    }
}