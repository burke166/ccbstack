using CcbStack.Core.Execution;
using CcbStack.Core.IO;
using CcbStack.Core.Repositories.Model;
using CcbStack.Core.Runtime;

namespace CcbStack.Core.Repositories;

/// <summary>
/// The default <see cref="IGitInspector"/>. Minimizes git process executions: a single
/// <c>git status --porcelain=v2 --branch</c> call yields the branch, detached-HEAD state,
/// commit hash, and staged/modified/untracked counts in one shot. Remote origin and default
/// branch each need one further best-effort call. Every git invocation is optional — a
/// missing git executable, a non-git directory, or a failing git command all degrade
/// gracefully instead of throwing.
/// </summary>
public sealed class GitInspector : IGitInspector
{
    private readonly IExecutableResolver _executableResolver;
    private readonly IProcessRunner _processRunner;
    private readonly IFileSystem _fileSystem;

    public GitInspector(IExecutableResolver executableResolver, IProcessRunner processRunner, IFileSystem fileSystem)
    {
        _executableResolver = executableResolver;
        _processRunner = processRunner;
        _fileSystem = fileSystem;
    }

    public async Task<GitInfo> InspectAsync(string repositoryRoot, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(repositoryRoot);

        if (!HasGitMarker(repositoryRoot))
        {
            return GitInfo.NotAGitRepository;
        }

        var git = await _executableResolver.ResolveAsync("git", cancellationToken).ConfigureAwait(false);
        if (git is null)
        {
            return GitInfo.GitUnavailable;
        }

        var statusResult = await RunGitAsync(git.FullPath, repositoryRoot, ["status", "--porcelain=v2", "--branch"], cancellationToken)
            .ConfigureAwait(false);
        if (statusResult is null || !statusResult.Succeeded)
        {
            return GitInfo.GitUnavailable;
        }

        var status = GitStatusParser.Parse(statusResult.StandardOutput);

        var remoteOriginUrl = await TryRunAsync(git.FullPath, repositoryRoot, ["remote", "get-url", "origin"], cancellationToken)
            .ConfigureAwait(false);
        var defaultBranchRef = await TryRunAsync(git.FullPath, repositoryRoot, ["symbolic-ref", "--short", "refs/remotes/origin/HEAD"], cancellationToken)
            .ConfigureAwait(false);

        return new GitInfo(
            IsGitRepository: true,
            Branch: status.Branch,
            IsDetachedHead: status.IsDetachedHead,
            IsDirty: status.StagedFileCount > 0 || status.ModifiedFileCount > 0 || status.UntrackedFileCount > 0,
            RemoteOriginUrl: remoteOriginUrl,
            DefaultBranch: StripOriginPrefix(defaultBranchRef),
            CommitHash: status.CommitHash,
            StagedFileCount: status.StagedFileCount,
            ModifiedFileCount: status.ModifiedFileCount,
            UntrackedFileCount: status.UntrackedFileCount);
    }

    private bool HasGitMarker(string repositoryRoot)
    {
        var markerPath = Path.Combine(repositoryRoot, ".git");
        return _fileSystem.FileExists(markerPath) || _fileSystem.DirectoryExists(markerPath);
    }

    private async Task<string?> TryRunAsync(string gitPath, string repositoryRoot, IReadOnlyList<string> arguments, CancellationToken cancellationToken)
    {
        var result = await RunGitAsync(gitPath, repositoryRoot, arguments, cancellationToken).ConfigureAwait(false);
        if (result is null || !result.Succeeded)
        {
            return null;
        }

        var trimmed = result.StandardOutput.Trim();
        return trimmed.Length == 0 ? null : trimmed;
    }

    private async Task<ProcessExecutionResult?> RunGitAsync(string gitPath, string repositoryRoot, IReadOnlyList<string> arguments, CancellationToken cancellationToken)
    {
        try
        {
            var request = new ProcessExecutionRequest(gitPath, arguments, repositoryRoot, Timeout: TimeSpan.FromSeconds(10));
            return await _processRunner.RunAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // The per-call timeout fired, not the caller's token; treat as a failed probe.
            return null;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            // git could not be started despite having just been resolved on PATH.
            return null;
        }
    }

    private static string? StripOriginPrefix(string? defaultBranchRef)
    {
        if (defaultBranchRef is null)
        {
            return null;
        }

        const string prefix = "origin/";
        return defaultBranchRef.StartsWith(prefix, StringComparison.Ordinal)
            ? defaultBranchRef[prefix.Length..]
            : defaultBranchRef;
    }
}
