namespace CcbStack.Core.Repositories.Model;

/// <summary>
/// The inferred purpose of a single .NET project file. Maps roughly one-to-one onto
/// <see cref="ApplicationClassification"/> via <c>DotNetApplicationClassificationRule</c>,
/// except <see cref="Test"/> and <see cref="Unknown"/>, which contribute no classification.
/// </summary>
public enum DotNetProjectKind
{
    Unknown,
    Library,
    Console,
    Cli,
    AspNetCoreWebApp,
    MinimalApi,
    Blazor,
    Worker,
    Desktop,
    Test,
}
