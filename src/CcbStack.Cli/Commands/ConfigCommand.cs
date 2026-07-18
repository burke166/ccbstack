using System.ComponentModel;
using CcbStack.Cli.Output;
using CcbStack.Cli.Rendering;
using CcbStack.Core.Configuration;
using CcbStack.Core.Runtime;
using Spectre.Console.Cli;

namespace CcbStack.Cli.Commands;

/// <summary>
/// Displays the effective ccbstack configuration, loaded from defaults, user and project
/// configuration files, environment variables, and command-line overrides.
/// </summary>
public sealed class ConfigCommand : AsyncCommand<ConfigCommand.Settings>
{
    private readonly ICcbStackConfigurationService _configurationService;
    private readonly IExecutableResolver _executableResolver;
    private readonly ICommandOutput _output;

    public ConfigCommand(
        ICcbStackConfigurationService configurationService,
        IExecutableResolver executableResolver,
        ICommandOutput output)
    {
        _configurationService = configurationService;
        _executableResolver = executableResolver;
        _output = output;
    }

    public sealed class Settings : CommandSettings
    {
        [CommandOption("--json")]
        [Description("Outputs the effective configuration as JSON instead of human-readable text.")]
        public bool Json { get; init; }

        [CommandOption("--config-json <JSON>")]
        [Description("Supplies a temporary, highest-precedence JSON configuration override. Mutually exclusive with --config-file.")]
        public string? ConfigJson { get; init; }

        [CommandOption("--config-file <PATH>")]
        [Description("Supplies a temporary, highest-precedence configuration override from a JSON file. Mutually exclusive with --config-json.")]
        public string? ConfigFile { get; init; }
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        // Mutual exclusion is checked here (not via Settings.Validate()) so this command can
        // return its own documented exit code rather than Spectre's built-in validation
        // failure code.
        if (settings.ConfigJson is not null && settings.ConfigFile is not null)
        {
            _output.WriteError(new CommandError("--config-json and --config-file are mutually exclusive."));
            return CcbStackExitCodes.InvalidCommandLine;
        }

        try
        {
            var input = new CommandLineConfigurationInput(settings.ConfigJson, settings.ConfigFile);
            var result = await _configurationService.LoadAsync(input, cancellationToken).ConfigureAwait(false);

            if (result.Diagnostics.Count > 0)
            {
                _output.WriteDiagnostics(result.Diagnostics);
            }

            if (!result.IsSuccess || result.Configuration is null)
            {
                return CcbStackExitCodes.InvalidConfiguration;
            }

            var view = await BuildViewAsync(result.Configuration, cancellationToken).ConfigureAwait(false);

            if (settings.Json)
            {
                _output.WriteObject(view);
            }
            else
            {
                _output.WriteText(ConfigOutputRenderer.ToText(view));
            }

            return CcbStackExitCodes.Success;
        }
        catch (Exception ex)
        {
            _output.WriteError(new CommandError($"ccbstack config failed unexpectedly: {ex.Message}", ex));
            return CcbStackExitCodes.UnexpectedError;
        }
    }

    private async Task<ConfigView> BuildViewAsync(CcbStackConfiguration configuration, CancellationToken cancellationToken)
    {
        var powerShell = await _executableResolver.ResolveAsync("pwsh", cancellationToken).ConfigureAwait(false);
        var git = await _executableResolver.ResolveAsync("git", cancellationToken).ConfigureAwait(false);
        var claude = await _executableResolver.ResolveAsync("claude", cancellationToken).ConfigureAwait(false);

        var runtime = new RuntimeEnvironmentView(powerShell?.FullPath, git?.FullPath, claude?.FullPath);
        return new ConfigView(configuration, runtime);
    }
}
