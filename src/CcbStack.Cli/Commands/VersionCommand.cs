using System.ComponentModel;
using System.Threading;
using CcbStack.Cli.Rendering;
using CcbStack.Core.Diagnostics;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CcbStack.Cli.Commands;

/// <summary>
/// Displays the ccbstack version, .NET runtime version, and operating system.
/// </summary>
public sealed class VersionCommand : Command<VersionCommand.Settings>
{
    private const string TextFormat = "text";
    private const string JsonFormat = "json";

    private readonly IAnsiConsole _console;
    private readonly VersionInfoProvider _versionInfoProvider;

    public VersionCommand(IAnsiConsole console)
    {
        _console = console;
        _versionInfoProvider = new VersionInfoProvider();
    }

    public sealed class Settings : CommandSettings
    {
        [CommandOption("--format <FORMAT>")]
        [Description("Output format. Allowed values: text (default) or json.")]
        [DefaultValue(TextFormat)]
        public string Format { get; init; } = TextFormat;

        public override ValidationResult Validate()
        {
            return Format.Equals(TextFormat, StringComparison.OrdinalIgnoreCase) ||
                   Format.Equals(JsonFormat, StringComparison.OrdinalIgnoreCase)
                ? ValidationResult.Success()
                : ValidationResult.Error(
                    $"Unsupported --format value '{Format}'. Expected '{TextFormat}' or '{JsonFormat}'.");
        }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellation)
    {
        var versionInfo = _versionInfoProvider.GetVersionInfo();

        var output = settings.Format.Equals(JsonFormat, StringComparison.OrdinalIgnoreCase)
            ? VersionOutputFormatter.ToJson(versionInfo)
            : VersionOutputFormatter.ToText(versionInfo);

        _console.WriteLine(output);

        return 0;
    }
}
