using CcbStack.Cli.Commands;

namespace CcbStack.Cli.Rendering;

/// <summary>Renders a <see cref="ConfigView"/> as human-readable terminal text.</summary>
internal static class ConfigOutputRenderer
{
    public static string ToText(ConfigView view)
    {
        var lines = new List<string>
        {
            $"Default Model     : {view.Configuration.DefaultModel}",
            $"Skills Directory  : {view.Configuration.SkillsDirectory}",
            $"Output Format     : {view.Configuration.Output.Format}",
            string.Empty,
            "Runtime Environment",
            $"PowerShell        : {view.Runtime.PowerShellPath ?? "not found"}",
            $"Git               : {view.Runtime.GitPath ?? "not found"}",
            $"Claude            : {view.Runtime.ClaudePath ?? "not found"}",
        };

        return string.Join(Environment.NewLine, lines);
    }
}
