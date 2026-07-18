using System.ComponentModel;
using CcbStack.Cli.Output;
using CcbStack.Cli.Rendering;
using CcbStack.Core.Repositories;
using CcbStack.Core.Runtime;
using Spectre.Console.Cli;

namespace CcbStack.Cli.Commands.Repo;

/// <summary>Displays repository intelligence: git status, languages, .NET/Go/PowerShell detection, and application classification.</summary>
public sealed class RepoInspectCommand : AsyncCommand<RepoInspectCommand.Settings>
{
    private readonly IRepositoryAnalyzer _repositoryAnalyzer;
    private readonly IRuntimeEnvironment _runtimeEnvironment;
    private readonly ICommandOutput _output;

    public RepoInspectCommand(IRepositoryAnalyzer repositoryAnalyzer, IRuntimeEnvironment runtimeEnvironment, ICommandOutput output)
    {
        _repositoryAnalyzer = repositoryAnalyzer;
        _runtimeEnvironment = runtimeEnvironment;
        _output = output;
    }

    public sealed class Settings : CommandSettings
    {
        [CommandOption("--json")]
        [Description("Outputs repository information as JSON instead of human-readable text.")]
        public bool Json { get; init; }

        [CommandOption("--path <PATH>")]
        [Description("Directory to start repository discovery from. Defaults to the current directory.")]
        public string? Path { get; init; }
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        try
        {
            var startingDirectory = settings.Path is not null
                ? System.IO.Path.GetFullPath(settings.Path)
                : _runtimeEnvironment.CurrentDirectory;

            var repository = await _repositoryAnalyzer.AnalyzeAsync(startingDirectory, cancellationToken).ConfigureAwait(false);

            if (settings.Json)
            {
                _output.WriteObject(repository);
            }
            else
            {
                _output.WriteText(RepositoryOutputRenderer.ToText(repository));
            }

            return CcbStackExitCodes.Success;
        }
        catch (Exception ex)
        {
            _output.WriteError(new CommandError($"ccbstack repo inspect failed unexpectedly: {ex.Message}", ex));
            return CcbStackExitCodes.UnexpectedError;
        }
    }
}
