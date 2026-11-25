using System.Text;

static class TextAlgorithms
{
    private const string Alphabet = "абвгґдеєжзиіїйклмнопрстуфхцчшщьюя";

    public static string[] Words(string text)
    {
        var words = new List<string>();
        var word = new StringBuilder();
        for (int i = 0; i < text.Length; i++)
        {
            char ch = text[i];
            if (char.IsLetter(ch))
                word.Append(char.ToLowerInvariant(ch));
            else if (word.Length > 0 && i + 1 < text.Length && char.IsLetter(text[i + 1]) &&
                     (ch is '\'' or '’' or 'ʼ' or '-'))
                word.Append(ch == '-' ? '-' : '’');
            else if (word.Length > 0)
            {
                words.Add(word.ToString());
                word.Clear();
            }
        }
        if (word.Length > 0)
            words.Add(word.ToString());
        return words.ToArray();
    }

    public static (string Word, int Count)[] MostFrequentSlow(string text, int limit)
    {
        var words = Words(text);
        var counted = new HashSet<string>(StringComparer.Ordinal);
        var results = new List<(string Word, int Count)>();
        foreach (string word in words)
        {
            if (!counted.Add(word))
                continue;
            int count = 0;
            foreach (string other in words)
                if (string.Equals(word, other, StringComparison.Ordinal))
                    count++;
            results.Add((word, count));
        }
        return Top(results, limit);
    }

    public static (string Word, int Count)[] MostFrequentFast(string text, int limit)
    {
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (string word in Words(text))
        {
            counts.TryGetValue(word, out int count);
            counts[word] = count + 1;
        }
        var results = new List<(string Word, int Count)>();
        foreach (var pair in counts)
            results.Add((pair.Key, pair.Value));
        return Top(results, limit);
    }

    public static void CheckTop((string Word, int Count)[] first, (string Word, int Count)[] second)
    {
        if (first.Length != second.Length)
            throw new InvalidOperationException("Кількість слів різна.");
        for (int i = 0; i < first.Length; i++)
            if (first[i] != second[i])
                throw new InvalidOperationException("Слова або частоти різняться.");
    }

    private static (string Word, int Count)[] Top(List<(string Word, int Count)> results, int limit)
    {
        var sorted = results.ToArray();
        Array.Sort(sorted, (left, right) =>
        {
            int byCount = right.Count.CompareTo(left.Count);
            return byCount != 0 ? byCount : string.Compare(left.Word, right.Word, StringComparison.Ordinal);
        });
        return sorted[..Math.Min(limit, sorted.Length)];
    }

    public static int IndexOf(string text, string pattern, out int comparisons)
    {
        comparisons = 0;
        for (int i = 0; i <= text.Length - pattern.Length; i++)
        {
            int j = 0;
            while (j < pattern.Length)
            {
                comparisons++;
                if (text[i + j] != pattern[j])
                    break;
                j++;
            }
            if (j == pattern.Length)
                return i;
        }
        return -1;
    }

    public static int[] LetterArray(string text)
    {
        var result = new int[Alphabet.Length];
        foreach (char letter in text)
        {
            int index = Alphabet.IndexOf(char.ToLowerInvariant(letter));
            if (index >= 0)
                result[index]++;
        }
        return result;
    }

    public static Dictionary<char, int> LetterDictionary(string text)
    {
        var result = new Dictionary<char, int>();
        foreach (char letter in text)
        {
            char normalized = char.ToLowerInvariant(letter);
            if (Alphabet.IndexOf(normalized) < 0)
                continue;
            result.TryGetValue(normalized, out int count);
            result[normalized] = count + 1;
        }
        return result;
    }

    public static void CheckLetters(int[] counts, Dictionary<char, int> other)
    {
        for (int i = 0; i < Alphabet.Length; i++)
        {
            other.TryGetValue(Alphabet[i], out int actual);
            if (counts[i] != actual)
                throw new InvalidOperationException("Частоти літер відрізняються.");
        }
    }
}
