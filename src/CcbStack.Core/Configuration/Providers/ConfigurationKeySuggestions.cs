namespace CcbStack.Core.Configuration.Providers;

/// <summary>Suggests a likely-intended configuration key for an unrecognized one, via edit distance.</summary>
internal static class ConfigurationKeySuggestions
{
    private const int MaxSuggestionDistance = 2;

    public static string? FindClosestMatch(string unknownKey, IEnumerable<string> knownKeys)
    {
        string? best = null;
        var bestDistance = int.MaxValue;

        foreach (var candidate in knownKeys)
        {
            var distance = LevenshteinDistance(unknownKey, candidate);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = candidate;
            }
        }

        return bestDistance <= MaxSuggestionDistance ? best : null;
    }

    private static int LevenshteinDistance(string a, string b)
    {
        var left = a.ToLowerInvariant();
        var right = b.ToLowerInvariant();
        var costs = new int[right.Length + 1];

        for (var j = 0; j <= right.Length; j++)
        {
            costs[j] = j;
        }

        for (var i = 1; i <= left.Length; i++)
        {
            costs[0] = i;
            var previousDiagonal = i - 1;

            for (var j = 1; j <= right.Length; j++)
            {
                var previousDiagonalSave = costs[j];
                costs[j] = left[i - 1] == right[j - 1]
                    ? previousDiagonal
                    : 1 + Math.Min(previousDiagonal, Math.Min(costs[j], costs[j - 1]));
                previousDiagonal = previousDiagonalSave;
            }
        }

        return costs[right.Length];
    }
}
