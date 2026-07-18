namespace CcbStack.Core.Repositories.Model;

/// <summary>
/// A possible application shape inferred from repository evidence. A repository can carry
/// multiple classifications at once (e.g. a CLI tool that is also a library), and the list
/// is intentionally open to growth — add a value and an
/// <c>IApplicationClassificationRule</c> that produces it, without touching existing rules.
/// </summary>
public enum ApplicationClassification
{
    Unknown,
    ConsoleTool,
    Cli,
    AspNetCoreWebApp,
    MinimalApi,
    Blazor,
    Library,
    WorkerService,
    DesktopApp,
    PowerShellModule,
    GoCli,
    MixedRepository,
}
