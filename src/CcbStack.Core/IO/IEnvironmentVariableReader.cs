namespace CcbStack.Core.IO;

/// <summary>
/// Reads process environment variables without ever mutating them. Kept behind an
/// abstraction so tests can supply a fixed set of variables instead of depending on the
/// real process environment.
/// </summary>
public interface IEnvironmentVariableReader
{
    IReadOnlyDictionary<string, string?> GetVariables();
}
