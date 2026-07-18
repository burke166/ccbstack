namespace CcbStack.Core.Repositories;

/// <summary>Parses <c>git status --porcelain=v2 --branch</c> output. See git-status(1), "Porcelain Format Version 2".</summary>
internal static class GitStatusParser
{
    private const string OidPrefix = "# branch.oid ";
    private const string HeadPrefix = "# branch.head ";
    private const string InitialOid = "(initial)";
    private const string DetachedHead = "(detached)";

    public static GitStatusInfo Parse(string statusOutput)
    {
        string? commitHash = null;
        string? branch = null;
        var detached = false;
        var staged = 0;
        var modified = 0;
        var untracked = 0;

        foreach (var rawLine in statusOutput.Split('\n'))
        {
            var line = rawLine.TrimEnd('\r');
            if (line.Length == 0)
            {
                continue;
            }

            if (line.StartsWith(OidPrefix, StringComparison.Ordinal))
            {
                var value = line[OidPrefix.Length..];
                commitHash = value == InitialOid ? null : value;
            }
            else if (line.StartsWith(HeadPrefix, StringComparison.Ordinal))
            {
                var value = line[HeadPrefix.Length..];
                if (value == DetachedHead)
                {
                    detached = true;
                }
                else
                {
                    branch = value;
                }
            }
            else if (line.StartsWith("1 ", StringComparison.Ordinal) || line.StartsWith("2 ", StringComparison.Ordinal))
            {
                if (TryExtractIndexAndWorkTreeStatus(line, out var indexStatus, out var workTreeStatus))
                {
                    if (indexStatus != '.')
                    {
                        staged++;
                    }

                    if (workTreeStatus != '.')
                    {
                        modified++;
                    }
                }
            }
            else if (line.StartsWith("u ", StringComparison.Ordinal))
            {
                modified++;
            }
            else if (line.StartsWith("? ", StringComparison.Ordinal))
            {
                untracked++;
            }
        }

        return new GitStatusInfo(commitHash, branch, detached, staged, modified, untracked);
    }

    private static bool TryExtractIndexAndWorkTreeStatus(string line, out char indexStatus, out char workTreeStatus)
    {
        // "1 XY N... <path>" / "2 XY N... <path>\t<origPath>" — XY starts right after the
        // leading "<kind> " marker.
        if (line.Length < 4)
        {
            indexStatus = '.';
            workTreeStatus = '.';
            return false;
        }

        indexStatus = line[2];
        workTreeStatus = line[3];
        return true;
    }
}

internal sealed record GitStatusInfo(
    string? CommitHash,
    string? Branch,
    bool IsDetachedHead,
    int StagedFileCount,
    int ModifiedFileCount,
    int UntrackedFileCount);
