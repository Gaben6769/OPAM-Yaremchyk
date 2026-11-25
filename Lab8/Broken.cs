static class Broken
{
    public static int SkipLast<T>(T[] values, T target, Func<T, T, int> cmp)
    {
        int lo = 0, hi = values.Length - 1;
        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2;
            int order = cmp(values[mid], target);
            if (order == 0)
                return mid;
            if (order < 0)
                lo = mid + 1;
            else
                hi = mid - 1;
        }
        return -1;
    }

    public static (int Lo, int Hi) Stuck<T>(T[] values, T target, Func<T, T, int> cmp)
    {
        int lo = 0, hi = values.Length - 1;
        for (int step = 0; lo <= hi && step < 10; step++)
        {
            int mid = lo + (hi - lo) / 2;
            int order = cmp(values[mid], target);
            if (order == 0)
                return (mid, mid);
            if (order < 0)
                lo = mid;
            else
                hi = mid - 1;
        }
        return (lo, hi);
    }

    public static int OverflowingMidpoint(int lo, int hi) => unchecked((lo + hi) / 2);

    public static int SearchUnsorted<T>(T[] values, T target, Func<T, T, int> cmp) =>
        Search.BinarySearch(values, target, cmp, out _);
}
