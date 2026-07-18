using System.Collections;

namespace CcbStack.Core.IO;

/// <summary>
/// The real <see cref="IEnvironmentVariableReader"/> implementation, backed by
/// <see cref="Environment.GetEnvironmentVariables()"/>. Variable names are matched
/// case-insensitively on Windows and with the platform's normal (case-sensitive) behavior
/// elsewhere.
/// </summary>
public sealed class EnvironmentVariableReader : IEnvironmentVariableReader
{
    public IReadOnlyDictionary<string, string?> GetVariables()
    {
        var comparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        var variables = new Dictionary<string, string?>(comparer);

        foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            var key = (string)entry.Key;
            variables[key] = (string?)entry.Value;
        }

        return variables;
    }
}
