using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Checks;

public interface IDoctorService
{
    DoctorReport Evaluate(RepositoryInfo repository);
}
