static class Algorithms
{
    public static List<T> Filter<T>(List<T> source, Func<T, bool> keep)
    {
        var result = new List<T>();
        foreach (T item in source)
            if (keep(item))
                result.Add(item);
        return result;
    }

    public static List<TOut> Map<TIn, TOut>(List<TIn> source, Func<TIn, TOut> convert)
    {
        var result = new List<TOut>(source.Count);
        foreach (TIn item in source)
            result.Add(convert(item));
        return result;
    }

    public static TAcc Aggregate<T, TAcc>(List<T> source, TAcc start, Func<TAcc, T, TAcc> step)
    {
        foreach (T item in source)
            start = step(start, item);
        return start;
    }

    public static Dictionary<TKey, List<T>> GroupBy<T, TKey>(List<T> source, Func<T, TKey> keyOf)
        where TKey : notnull
    {
        var result = new Dictionary<TKey, List<T>>();
        foreach (T item in source)
        {
            TKey key = keyOf(item);
            if (!result.TryGetValue(key, out var group))
            {
                group = new List<T>();
                result.Add(key, group);
            }
            group.Add(item);
        }
        return result;
    }
}
