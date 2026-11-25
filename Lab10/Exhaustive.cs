static partial class RodCutting
{
    public static int Exhaustive(int rod, int[] pieces)
    {
        int best = rod;
        Visit(0, rod);
        return best;

        void Visit(int index, int remaining)
        {
            if (index == pieces.Length)
            {
                best = Math.Min(best, remaining);
                return;
            }
            Visit(index + 1, remaining);
            if (pieces[index] <= remaining)
                Visit(index + 1, remaining - pieces[index]);
        }
    }
}
