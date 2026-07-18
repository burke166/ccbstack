using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories.Classification;

public sealed class PowerShellApplicationClassificationRule : IApplicationClassificationRule
{
    public IReadOnlyCollection<ApplicationClassification> Classify(RepositoryEvidence evidence)
    {
        return evidence.PowerShell.HasModules ? [ApplicationClassification.PowerShellModule] : [];
    }
}
