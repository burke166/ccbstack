namespace CcbStack.Core.Checks;

public sealed record DoctorCheckResult(string Name, DoctorCheckStatus Status, string? Detail = null);
