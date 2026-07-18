using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Checks;

/// <summary>
/// Runs every registered <see cref="IDoctorCheck"/> against an already-computed
/// <see cref="RepositoryInfo"/>. Contains no repository-inspection logic of its own —
/// <c>doctor</c> and <c>repo inspect</c> share the exact same analysis.
/// </summary>
public sealed class DoctorService : IDoctorService
{
    private readonly IEnumerable<IDoctorCheck> _checks;

    public DoctorService(IEnumerable<IDoctorCheck> checks)
    {
        _checks = checks;
    }

    public DoctorReport Evaluate(RepositoryInfo repository)
    {
        var results = _checks
            .Select(check => check.Evaluate(repository))
            .Where(result => result is not null)
            .Select(result => result!)
            .ToList();

        return new DoctorReport(results);
    }
}
