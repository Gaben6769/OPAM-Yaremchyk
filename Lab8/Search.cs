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
