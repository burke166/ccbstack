using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories.Classification;

public sealed class GoApplicationClassificationRule : IApplicationClassificationRule
{
    public IReadOnlyCollection<ApplicationClassification> Classify(RepositoryEvidence evidence)
    {
        if (!evidence.Go.HasGoMod)
        {
            return [];
        }

        return evidence.Go.HasCmdDirectory
            ? [ApplicationClassification.GoCli]
            : [ApplicationClassification.Library];
    }
}
