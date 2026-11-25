static class Checks
{
    public static bool IsSorted<T>(T[] values, Func<T, T, int> cmp)
    {
        for (int i = 1; i < values.Length; i++)
            if (cmp(values[i - 1], values[i]) > 0)
                return false;
        return true;
    }

    public static bool IsPermutationOf<T>(T[] actual, T[] source) where T : notnull
    {
        if (actual.Length != source.Length)
            return false;
        var counts = new Dictionary<T, int>();
        foreach (var item in source)
        {
            counts.TryGetValue(item, out int count);
            counts[item] = count + 1;
        }
        foreach (var item in actual)
        {
            if (!counts.TryGetValue(item, out int count) || count == 0)
                return false;
            counts[item] = count - 1;
        }
        return true;
    }

    public static void Validate<T>(T[] actual, T[] source, Func<T, T, int> cmp) where T : notnull
    {
        if (!IsSorted(actual, cmp) || !IsPermutationOf(actual, source))
            throw new InvalidOperationException("Результат сортування некоректний.");
    }
}
