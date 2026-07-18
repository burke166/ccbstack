namespace CcbStack.Core.Repositories.Model;

/// <summary>PowerShell script/module evidence gathered from the repository.</summary>
public sealed record PowerShellInfo(
    int ScriptFileCount,
    int ModuleFileCount,
    bool HasBuildScripts)
{
    public static PowerShellInfo NotDetected { get; } = new(0, 0, false);

    public bool HasScripts => ScriptFileCount > 0;

    public bool HasModules => ModuleFileCount > 0;

    public bool IsDetected => HasScripts || HasModules;
}
