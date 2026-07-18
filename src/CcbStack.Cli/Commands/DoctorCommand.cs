using System.ComponentModel;
using CcbStack.Cli.Output;
using CcbStack.Cli.Rendering;
using CcbStack.Core.Checks;
using CcbStack.Core.Repositories;
using CcbStack.Core.Runtime;
using Spectre.Console.Cli;

namespace CcbStack.Cli.Commands;

/// <summary>Runs basic repository health checks against the same repository model <c>repo inspect</c> uses.</summary>
public sealed class DoctorCommand : AsyncCommand<DoctorCommand.Settings>
{
    private readonly IRepositoryAnalyzer _repositoryAnalyzer;
    private readonly IDoctorService _doctorService;
    private readonly IRuntimeEnvironment _runtimeEnvironment;
    private readonly ICommandOutput _output;

    public DoctorCommand(
        IRepositoryAnalyzer repositoryAnalyzer,
        IDoctorService doctorService,
        IRuntimeEnvironment runtimeEnvironment,
        ICommandOutput output)
    {
        _repositoryAnalyzer = repositoryAnalyzer;
        _doctorService = doctorService;
        _runtimeEnvironment = runtimeEnvironment;
        _output = output;
    }

    public sealed class Settings : CommandSettings
    {
        [CommandOption("--json")]
        [Description("Outputs the doctor report as JSON instead of human-readable text.")]
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
            var report = _doctorService.Evaluate(repository);

            if (settings.Json)
            {
                _output.WriteObject(report);
            }
            else
            {
                _output.WriteText(DoctorOutputRenderer.ToText(report));
            }

            return report.HasFailures ? CcbStackExitCodes.CheckFailed : CcbStackExitCodes.Success;
        }
        catch (Exception ex)
        {
            _output.WriteError(new CommandError($"ccbstack doctor failed unexpectedly: {ex.Message}", ex));
            return CcbStackExitCodes.UnexpectedError;
        }
    }
}
