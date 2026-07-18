using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories.Classification;

/// <summary>Maps each detected .NET project's <see cref="DotNetProjectKind"/> onto an <see cref="ApplicationClassification"/>.</summary>
public sealed class DotNetApplicationClassificationRule : IApplicationClassificationRule
{
    private static readonly IReadOnlyDictionary<DotNetProjectKind, ApplicationClassification> KindMap =
        new Dictionary<DotNetProjectKind, ApplicationClassification>
        {
            [DotNetProjectKind.Console] = ApplicationClassification.ConsoleTool,
            [DotNetProjectKind.Cli] = ApplicationClassification.Cli,
            [DotNetProjectKind.AspNetCoreWebApp] = ApplicationClassification.AspNetCoreWebApp,
            [DotNetProjectKind.MinimalApi] = ApplicationClassification.MinimalApi,
            [DotNetProjectKind.Blazor] = ApplicationClassification.Blazor,
            [DotNetProjectKind.Worker] = ApplicationClassification.WorkerService,
            [DotNetProjectKind.Desktop] = ApplicationClassification.DesktopApp,
            [DotNetProjectKind.Library] = ApplicationClassification.Library,
        };

    public IReadOnlyCollection<ApplicationClassification> Classify(RepositoryEvidence evidence)
    {
        var results = new HashSet<ApplicationClassification>();

        foreach (var project in evidence.DotNet.Projects)
        {
            if (KindMap.TryGetValue(project.Kind, out var classification))
            {
                results.Add(classification);
            }
        }

        return results;
    }
}
