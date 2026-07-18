using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories.Classification;

public interface IApplicationClassifier
{
    IReadOnlyList<ApplicationClassification> Classify(RepositoryEvidence evidence);
}
