using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories.Classification;

/// <summary>
/// Contributes zero or more <see cref="ApplicationClassification"/> values inferred from
/// repository evidence. Adding a new classification (Docker, a new .NET project shape, ...)
/// means registering another implementation, never editing an existing one.
/// </summary>
public interface IApplicationClassificationRule
{
    IReadOnlyCollection<ApplicationClassification> Classify(RepositoryEvidence evidence);
}
