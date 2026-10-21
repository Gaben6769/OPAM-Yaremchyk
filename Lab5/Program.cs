using System;
using System.Collections.Generic;

class Program
{
    const int N = 31;
    const int K = 33;

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine($"=== Лабораторна робота 5 (Варіант: N = {N}, K = {K}) ===\n");

        // --- Завдання 1 ---
        Console.WriteLine("--- Завдання 1: Збалансованість дужок () [] {} <> ---");
        string testK = new string('(', 33) + new string(')', 32);
        string[] testStrings = new string[]
        {
            "(a[b]{c})",
            "([)]",
            ")(",
            "((",
            "",
            testK
        };

        foreach (var str in testStrings)
        {
            var (isValid, errIdx) = CheckBracketsStandard(str);
            string displayStr = str.Length > 40 ? str.Substring(0, 15) + "..." + str.Substring(str.Length - 15) : str;
            Console.WriteLine($"Рядок: \"{displayStr}\" -> Збалансований: {isValid}, Індекс помилки: {errIdx}");
        }
        Console.WriteLine();

        // --- Завдання 2 ---
        Console.WriteLine("--- Завдання 2: Зворотний польський запис (+ - * / min max) ---");
        // Власний вираз із результатом 33 та щонайменше 3 операціями:
        // "10 50 min 23 + 0 max" -> min(10,50)=10; 10+23=33; max(33,0)=33
        string customExpr = "10 50 min 23 + 0 max";
        string[] rpnTests = new string[]
        {
            "3 4 + 2 *",
            "5 3 -",
            "8 2 /",
            "2 3 4 * +",
            "5 0 /",
            customExpr
        };

        foreach (var expr in rpnTests)
        {
            var (success, result, message) = EvaluateRPN(expr);
            if (success)
                Console.WriteLine($"Вираз: \"{expr}\" -> Результат: {result}");
            else
                Console.WriteLine($"Вираз: \"{expr}\" -> Помилка: {message}");
        }
        Console.WriteLine();

        // --- Завдання 3 ---
        Console.WriteLine("--- Завдання 3: Модель обслуговування (Схема 1) ---");
        SimulateServiceQueue(N, K);
        Console.WriteLine();

        // --- Завдання 4 ---
        Console.WriteLine("--- Завдання 4: Власний стек поверх масиву (IntStack) ---");
        TestIntStack();
        Console.WriteLine();

        // --- Перевірка Завдання 1 на власному CharStack ---
        Console.WriteLine("--- Завдання 1 на власному CharStack ---");
        foreach (var str in testStrings)
        {
            var (isValid, errIdx) = CheckBracketsCustom(str);
            string displayStr = str.Length > 40 ? str.Substring(0, 15) + "..." + str.Substring(str.Length - 15) : str;
            Console.WriteLine($"Рядок: \"{displayStr}\" -> Збалансований: {isValid}, Індекс помилки: {errIdx}");
        }
    }

    // ==========================================
    // ЗАВДАННЯ 1: Стандартний Stack<T>
    // ==========================================
    static (bool IsValid, int ErrorIndex) CheckBracketsStandard(string input)
    {
        Stack<(char Ch, int Index)> stack = new Stack<(char, int)>();

        for (int i = 0; i < input.Length; i++)
        {
            char ch = input[i];

            if (ch == '(' || ch == '[' || ch == '{' || ch == '<')
            {
                stack.Push((ch, i));
            }
            else if (ch == ')' || ch == ']' || ch == '}' || ch == '>')
            {
                if (stack.Count == 0)
                {
                    return (false, i); // Закривальна дужка при порожньому стеку
                }

                var top = stack.Pop();
                if (!IsMatchingPair(top.Ch, ch))
                {
                    return (false, i); // Невідповідність типів
                }
            }
        }

        if (stack.Count > 0)
        {
            // Непорожній стек після кінця рядка
            var remaining = stack.Pop();
            return (false, input.Length);
        }

        return (true, -1);
    }

    static bool IsMatchingPair(char open, char close)
    {
        return (open == '(' && close == ')') ||
               (open == '[' && close == ']') ||
               (open == '{' && close == '}') ||
               (open == '<' && close == '>');
    }

    // ==========================================
    // ЗАВДАННЯ 2: Зворотний польський запис
    // ==========================================
    static (bool Success, double Result, string Message) EvaluateRPN(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return (false, 0, "Порожній вираз");

        string[] tokens = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        Stack<double> stack = new Stack<double>();

        foreach (string token in tokens)
        {
            if (double.TryParse(token, out double number))
            {
                stack.Push(number);
            }
            else
            {
                if (stack.Count < 2)
                {
                    return (false, 0, $"Забракло операндів для операції '{token}'");
                }

                double b = stack.Pop();
                double a = stack.Pop();

                switch (token.ToLower())
                {
                    case "+":
                        stack.Push(a + b);
                        break;
                    case "-":
                        stack.Push(a - b);
                        break;
                    case "*":
                        stack.Push(a * b);
                        break;
                    case "/":
                        if (Math.Abs(b) < 1e-9)
                            return (false, 0, "Ділення на нуль");
                        stack.Push(a / b);
                        break;
                    case "min":
                        stack.Push(Math.Min(a, b));
                        break;
                    case "max":
                        stack.Push(Math.Max(a, b));
                        break;
                    default:
                        return (false, 0, $"Невідомий токен '{token}'");
                }
            }
        }

        if (stack.Count == 1)
        {
            return (true, stack.Pop(), "OK");
        }
        else if (stack.Count > 1)
        {
            return (false, 0, "Забагато чисел у виразі (недостатньо операцій)");
        }
        else
        {
            return (false, 0, "Вираз не повернув результат");
        }
    }

    // ==========================================
    // ЗАВДАННЯ 3: Черга обслуговування (Схема 1)
    // ==========================================
    struct Request
    {
        public int Id;
        public int ArrivalTime;
        public int Weight;

        public Request(int id, int arrivalTime, int weight)
        {
            Id = id;
            ArrivalTime = arrivalTime;
            Weight = weight;
        }
    }

    static void SimulateServiceQueue(int seedN, int seedK)
    {
        int totalRequestsCount = 8 + seedK % 5; // 8 + 3 = 11
        Random rnd = new Random(seedN);

        List<Request> allRequests = new List<Request>();
        int currentArrival = 0;

        for (int i = 1; i <= totalRequestsCount; i++)
        {
            currentArrival += rnd.Next(0, 4); // інтервал приходу 0..3 хв
            int weight = rnd.Next(1, 10);      // вага 1..9 хв
            allRequests.Add(new Request(i, currentArrival, weight));
        }

        Queue<Request> queue = new Queue<Request>();
        int currentTime = 0;
        int idleTime = 0;
        int maxQueueLength = 0;
        int requestIndex = 0;

        Console.WriteLine($"Кількість заявок: {totalRequestsCount}");
        Console.WriteLine("№\tПрихід\tВага\tПочаток\tКінець");

        while (requestIndex < allRequests.Count || queue.Count > 0)
        {
            while (requestIndex < allRequests.Count && allRequests[requestIndex].ArrivalTime <= currentTime)
            {
                queue.Enqueue(allRequests[requestIndex]);
                requestIndex++;
            }

            if (queue.Count > maxQueueLength)
                maxQueueLength = queue.Count;

            if (queue.Count > 0)
            {
                Request currentReq = queue.Dequeue();
                int startTime = currentTime;
                int endTime = startTime + currentReq.Weight;

                Console.WriteLine($"{currentReq.Id}\t{currentReq.ArrivalTime}\t{currentReq.Weight}\t{startTime}\t{endTime}");
                currentTime = endTime;
            }
            else
            {
                if (requestIndex < allRequests.Count)
                {
                    int nextArrival = allRequests[requestIndex].ArrivalTime;
                    idleTime += (nextArrival - currentTime);
                    currentTime = nextArrival;
                }
            }
        }

        Console.WriteLine($"Загальний час простою: {idleTime} хв");
        Console.WriteLine($"Максимальна довжина черги: {maxQueueLength}");
    }

    // ==========================================
    // ЗАВДАННЯ 4: Власний стек поверх масиву
    // ==========================================
    public class IntStack
    {
        private int[] items;
        private int count;

        public int Count => count;
        public int Capacity => items.Length;

        public IntStack(int initialCapacity = 3)
        {
            items = new int[initialCapacity];
            count = 0;
        }

        public void Push(int x)
        {
            if (count == items.Length)
            {
                Array.Resize(ref items, items.Length * 2);
            }
            items[count] = x;
            count++;
        }

        public int Pop()
        {
            if (count == 0)
                throw new InvalidOperationException("Стек порожній!");

            count--;
            return items[count];
        }

        public int Peek()
        {
            if (count == 0)
                throw new InvalidOperationException("Стек порожній!");

            return items[count - 1];
        }
    }

    public class CharStack
    {
        private (char Ch, int Index)[] items;
        private int count;

        public int Count => count;
        public int Capacity => items.Length;

        public CharStack(int initialCapacity = 3)
        {
            items = new (char, int)[initialCapacity];
            count = 0;
        }

        public void Push((char Ch, int Index) item)
        {
            if (count == items.Length)
            {
                Array.Resize(ref items, items.Length * 2);
            }
            items[count] = item;
            count++;
        }

        public (char Ch, int Index) Pop()
        {
            if (count == 0)
                throw new InvalidOperationException("Стек порожній!");

            count--;
            return items[count];
        }

        public (char Ch, int Index) Peek()
        {
            if (count == 0)
                throw new InvalidOperationException("Стек порожній!");

            return items[count - 1];
        }
    }

    static void TestIntStack()
    {
        int initialCap = K % 4 + 2; // 33 % 4 + 2 = 3
        int pushCalls = K % 4 + 3;  // 33 % 4 + 3 = 4

        Console.WriteLine($"Створення IntStack з початковою місткістю Capacity = {initialCap}");
        IntStack stack = new IntStack(initialCap);

        for (int i = 1; i <= pushCalls; i++)
        {
            stack.Push(i * 10);
            Console.WriteLine($"Після Push #{i}: Count = {stack.Count}, Capacity = {stack.Capacity}");
        }
    }

    static (bool IsValid, int ErrorIndex) CheckBracketsCustom(string input)
    {
        CharStack stack = new CharStack(3);

        for (int i = 0; i < input.Length; i++)
        {
            char ch = input[i];

            if (ch == '(' || ch == '[' || ch == '{' || ch == '<')
            {
                stack.Push((ch, i));
            }
            else if (ch == ')' || ch == ']' || ch == '}' || ch == '>')
            {
                if (stack.Count == 0)
                {
                    return (false, i);
                }

                var top = stack.Pop();
                if (!IsMatchingPair(top.Ch, ch))
                {
                    return (false, i);
                }
            }
        }

        if (stack.Count > 0)
        {
            var remaining = stack.Pop();
            return (false, input.Length);
        }

        return (true, -1);
    }
}