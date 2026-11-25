static partial class RodCutting
{
    public static int Greedy(int rod, int[] pieces)
    {
        var sorted = (int[])pieces.Clone();
        Array.Sort(sorted);
        int remaining = rod;
        for (int i = sorted.Length - 1; i >= 0; i--)
            if (sorted[i] <= remaining)
                remaining -= sorted[i];
        return remaining;
    }
}
