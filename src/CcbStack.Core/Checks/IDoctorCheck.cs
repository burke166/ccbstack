using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Checks;

/// <summary>
/// A single doctor check evaluated against the already-computed <see cref="RepositoryInfo"/>.
/// Returns <see langword="null"/> when the check does not apply to this repository (e.g. a
/// Go-module check on a repository with no <c>go.mod</c>), so it is silently omitted from the
/// report rather than reported as a false warning. Registering another implementation is the
/// supported way to add a check.
/// </summary>
public interface IDoctorCheck
{
    DoctorCheckResult? Evaluate(RepositoryInfo repository);
}
