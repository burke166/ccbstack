using System.Text;
using CcbStack.Core.Repositories.Model;

namespace CcbStack.Cli.Rendering;

/// <summary>Renders a <see cref="RepositoryInfo"/> as human-readable terminal text.</summary>
internal static class RepositoryOutputRenderer
{
    public static string ToText(RepositoryInfo repository)
    {
        var builder = new StringBuilder();

        builder.AppendLine("Repository").AppendLine();
        builder.AppendLine("  Root").AppendLine($"    {repository.RootPath}").AppendLine();

        AppendGitSection(builder, repository.Git);
        AppendLanguagesSection(builder, repository.Languages);
        AppendDotNetSection(builder, repository.DotNet);
        AppendGoSection(builder, repository.Go);
        AppendPowerShellSection(builder, repository.PowerShell);
        AppendClassificationSection(builder, repository.Applications);

        return builder.ToString().TrimEnd();
    }

    private static void AppendGitSection(StringBuilder builder, GitInfo git)
    {
        builder.AppendLine("Git").AppendLine();

        if (!git.IsGitRepository)
        {
            builder.AppendLine("  Not detected").AppendLine();
            return;
        }

        builder.AppendLine("  Branch").AppendLine($"    {(git.IsDetachedHead ? "(detached HEAD)" : git.Branch ?? "unknown")}").AppendLine();
        builder.AppendLine("  Status").AppendLine($"    {(git.IsDirty ? $"Dirty ({git.StagedFileCount} staged, {git.ModifiedFileCount} modified, {git.UntrackedFileCount} untracked)" : "Clean")}").AppendLine();

        if (git.CommitHash is not null)
        {
            builder.AppendLine("  Commit").AppendLine($"    {git.CommitHash}").AppendLine();
        }

        if (git.RemoteOriginUrl is not null)
        {
            builder.AppendLine("  Remote").AppendLine($"    {git.RemoteOriginUrl}").AppendLine();
        }

        if (git.DefaultBranch is not null)
        {
            builder.AppendLine("  Default Branch").AppendLine($"    {git.DefaultBranch}").AppendLine();
        }
    }

    private static void AppendLanguagesSection(StringBuilder builder, IReadOnlyList<RepositoryLanguage> languages)
    {
        builder.AppendLine("Languages").AppendLine();

        if (languages.Count == 0)
        {
            builder.AppendLine("  Not detected").AppendLine();
            return;
        }

        foreach (var language in languages)
        {
            builder.AppendLine($"  {FormatLanguage(language)}");
        }

        builder.AppendLine();
    }

    private static void AppendDotNetSection(StringBuilder builder, DotNetInfo dotNet)
    {
        builder.AppendLine(".NET").AppendLine();

        if (!dotNet.IsDetected)
        {
            builder.AppendLine("  Not detected").AppendLine();
            return;
        }

        builder.AppendLine("  SDK Projects").AppendLine($"    {dotNet.Projects.Count}").AppendLine();
        builder.AppendLine("  Solutions").AppendLine($"    {dotNet.SolutionFiles.Count}").AppendLine();

        if (dotNet.TargetFrameworks.Count > 0)
        {
            builder.AppendLine("  Target Frameworks").AppendLine();
            foreach (var targetFramework in dotNet.TargetFrameworks)
            {
                builder.AppendLine($"    {targetFramework}");
            }

            builder.AppendLine();
        }
    }

    private static void AppendGoSection(StringBuilder builder, GoInfo go)
    {
        builder.AppendLine("Go").AppendLine();

        if (!go.HasGoMod)
        {
            builder.AppendLine("  Not detected").AppendLine();
            return;
        }

        builder.AppendLine("  Module").AppendLine($"    {go.ModuleName ?? "unknown"}").AppendLine();

        if (go.GoVersion is not null)
        {
            builder.AppendLine("  Go Version").AppendLine($"    {go.GoVersion}").AppendLine();
        }
    }

    private static void AppendPowerShellSection(StringBuilder builder, PowerShellInfo powerShell)
    {
        builder.AppendLine("PowerShell").AppendLine();

        if (!powerShell.IsDetected)
        {
            builder.AppendLine("  Not detected").AppendLine();
            return;
        }

        builder.AppendLine("  Scripts").AppendLine($"    {powerShell.ScriptFileCount}").AppendLine();
        builder.AppendLine("  Modules").AppendLine($"    {powerShell.ModuleFileCount}").AppendLine();
    }

    private static void AppendClassificationSection(StringBuilder builder, IReadOnlyList<ApplicationClassification> applications)
    {
        builder.AppendLine("Classification").AppendLine();

        foreach (var application in applications)
        {
            builder.AppendLine($"  {FormatClassification(application)}");
        }
    }

    private static string FormatLanguage(RepositoryLanguage language) => language switch
    {
        RepositoryLanguage.CSharp => "C#",
        RepositoryLanguage.FSharp => "F#",
        RepositoryLanguage.VisualBasic => "VB.NET",
        RepositoryLanguage.Go => "Go",
        RepositoryLanguage.PowerShell => "PowerShell",
        RepositoryLanguage.JavaScript => "JavaScript",
        RepositoryLanguage.TypeScript => "TypeScript",
        _ => language.ToString(),
    };

    private static string FormatClassification(ApplicationClassification classification) => classification switch
    {
        ApplicationClassification.ConsoleTool => "Console Tool",
        ApplicationClassification.Cli => "CLI",
        ApplicationClassification.AspNetCoreWebApp => "ASP.NET Core Web App",
        ApplicationClassification.MinimalApi => "Minimal API",
        ApplicationClassification.Blazor => "Blazor",
        ApplicationClassification.Library => "Library",
        ApplicationClassification.WorkerService => "Worker Service",
        ApplicationClassification.DesktopApp => "Desktop App",
        ApplicationClassification.PowerShellModule => "PowerShell Module",
        ApplicationClassification.GoCli => "Go CLI",
        ApplicationClassification.MixedRepository => "Mixed Repository",
        _ => "Unknown",
    };
}
