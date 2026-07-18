namespace CcbStack.Core.Configuration;

/// <summary>
/// Validates a fully merged, materialized <see cref="CcbStackConfiguration"/>. Validation
/// runs once, after all provider sources have been merged — providers must not perform this
/// validation themselves.
/// </summary>
public interface ICcbStackConfigurationValidator
{
    IReadOnlyList<ConfigurationDiagnostic> Validate(CcbStackConfiguration configuration);
}
