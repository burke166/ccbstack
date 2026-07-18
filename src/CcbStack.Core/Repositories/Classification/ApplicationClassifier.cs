using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories.Classification;

/// <summary>
/// Aggregates every registered <see cref="IApplicationClassificationRule"/>, then adds
/// <see cref="ApplicationClassification.MixedRepository"/> when evidence spans more than one
/// language ecosystem, and falls back to <see cref="ApplicationClassification.Unknown"/> when
/// no rule contributed anything.
/// </summary>
public sealed class ApplicationClassifier : IApplicationClassifier
{
    private readonly IEnumerable<IApplicationClassificationRule> _rules;

    public ApplicationClassifier(IEnumerable<IApplicationClassificationRule> rules)
    {
        _rules = rules;
    }

    public IReadOnlyList<ApplicationClassification> Classify(RepositoryEvidence evidence)
    {
        var classifications = new HashSet<ApplicationClassification>();

        foreach (var rule in _rules)
        {
            foreach (var classification in rule.Classify(evidence))
            {
                classifications.Add(classification);
            }
        }

        if (classifications.Count == 0)
        {
            return [ApplicationClassification.Unknown];
        }

        if (CountLanguageEcosystems(evidence) > 1)
        {
            classifications.Add(ApplicationClassification.MixedRepository);
        }

        // Enum declaration order, not rule-registration order, so output is stable across runs.
        return Enum.GetValues<ApplicationClassification>().Where(classifications.Contains).ToList();
    }

    private static int CountLanguageEcosystems(RepositoryEvidence evidence)
    {
        var count = 0;
        if (evidence.DotNet.IsDetected) count++;
        if (evidence.Go.HasGoMod) count++;
        if (evidence.PowerShell.IsDetected) count++;
        return count;
    }
}
