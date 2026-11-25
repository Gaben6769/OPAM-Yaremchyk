static class Sorts
{
    public static void Insertion<T>(T[] values, Func<T, T, int> cmp, out int comparisons, out int moves)
    {
        long checkedPairs = 0, writes = 0;
        for (int i = 1; i < values.Length; i++)
        {
            T item = values[i];
            int pos = i;
            while (pos > 0)
            {
                checkedPairs++;
                if (cmp(values[pos - 1], item) <= 0)
                    break;
                values[pos] = values[pos - 1];
                writes++;
                pos--;
            }
            if (pos != i)
            {
                values[pos] = item;
                writes++;
            }
        }
        comparisons = Counter(checkedPairs);
        moves = Counter(writes);
    }

    public static void BinaryInsertion<T>(T[] values, Func<T, T, int> cmp, out int comparisons, out int moves)
    {
        long checkedPairs = 0, writes = 0;
        for (int i = 1; i < values.Length; i++)
        {
            T item = values[i];
            int lo = 0, hi = i;
            while (lo < hi)
            {
                int mid = lo + (hi - lo) / 2;
                checkedPairs++;
                if (cmp(values[mid], item) <= 0)
                    lo = mid + 1;
                else
                    hi = mid;
            }
            if (lo != i)
            {
                Array.Copy(values, lo, values, lo + 1, i - lo);
                values[lo] = item;
                writes += i - lo + 1;
            }
        }
        comparisons = Counter(checkedPairs);
        moves = Counter(writes);
    }

    public static void Quick<T>(T[] values, Func<T, T, int> cmp, out int comparisons, out int moves)
    {
        long checkedPairs = 0, writes = 0;
        SortRange(0, values.Length - 1);
        comparisons = Counter(checkedPairs);
        moves = Counter(writes);

        void SortRange(int first, int last)
        {
            while (first < last)
            {
                int middle = first + (last - first) / 2;
                int pivotIndex = MedianIndex(first, middle, last);
                T pivot = values[pivotIndex];
                Swap(pivotIndex, last);
                int next = first;
                for (int i = first; i < last; i++)
                    if (Compare(values[i], pivot) < 0)
                        Swap(next++, i);
                Swap(next, last);

                // Process the shorter side recursively to limit stack usage.
                if (next - first < last - next)
                {
                    SortRange(first, next - 1);
                    first = next + 1;
                }
                else
                {
                    SortRange(next + 1, last);
                    last = next - 1;
                }
            }
        }

        int MedianIndex(int left, int middle, int right)
        {
            if (Compare(values[left], values[middle]) > 0)
                (left, middle) = (middle, left);
            if (Compare(values[middle], values[right]) > 0)
                (middle, right) = (right, middle);
            if (Compare(values[left], values[middle]) > 0)
                middle = left;
            return middle;
        }

        int Compare(T left, T right)
        {
            checkedPairs++;
            return cmp(left, right);
        }

        void Swap(int left, int right)
        {
            if (left == right)
                return;
            (values[left], values[right]) = (values[right], values[left]);
            writes += 2;
        }
    }

    // A negative sentinel marks measurements too large for the assignment's out int signature.
    private static int Counter(long value) => value <= int.MaxValue ? (int)value : -1;
}
