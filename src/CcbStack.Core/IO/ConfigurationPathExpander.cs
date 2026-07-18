using CcbStack.Core.Runtime;

namespace CcbStack.Core.IO;

/// <summary>
/// Expands the documented path substitutions (<c>%USERPROFILE%</c>, <c>%APPDATA%</c>,
/// <c>%LOCALAPPDATA%</c>, and a leading <c>~</c>) and resolves relative paths against a
/// supplied base directory. Deliberately does not perform arbitrary shell expansion. Pure
/// and stateless, so no interface is needed for test substitution.
/// </summary>
public sealed class ConfigurationPathExpander
{
    /// <summary>
    /// Expands documented substitutions in <paramref name="path"/> using values from
    /// <paramref name="runtimeEnvironment"/>. Does not resolve relative paths — see
    /// <see cref="ResolveRelativePath"/>.
    /// </summary>
    public string ExpandSubstitutions(string path, IRuntimeEnvironment runtimeEnvironment)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(runtimeEnvironment);

        var expanded = path;

        if (expanded.StartsWith('~'))
        {
            expanded = runtimeEnvironment.UserProfileDirectory + expanded[1..];
        }

        expanded = ReplaceToken(expanded, "%USERPROFILE%", runtimeEnvironment.UserProfileDirectory);
        expanded = ReplaceToken(expanded, "%APPDATA%", runtimeEnvironment.AppDataDirectory);
        expanded = ReplaceToken(expanded, "%LOCALAPPDATA%", runtimeEnvironment.LocalAppDataDirectory);

        return expanded;
    }

    /// <summary>
    /// Resolves <paramref name="path"/> against <paramref name="baseDirectory"/> when it is
    /// relative; returns it unchanged when it is already rooted.
    /// </summary>
    public string ResolveRelativePath(string path, string baseDirectory)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(baseDirectory);

        return Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(baseDirectory, path));
    }

    /// <summary>Expands substitutions and then resolves the result relative to <paramref name="baseDirectory"/>.</summary>
    public string Expand(string path, string baseDirectory, IRuntimeEnvironment runtimeEnvironment)
    {
        var substituted = ExpandSubstitutions(path, runtimeEnvironment);
        return ResolveRelativePath(substituted, baseDirectory);
    }

    private static string ReplaceToken(string path, string token, string replacement)
    {
        return path.Replace(token, replacement, StringComparison.OrdinalIgnoreCase);
    }
}
