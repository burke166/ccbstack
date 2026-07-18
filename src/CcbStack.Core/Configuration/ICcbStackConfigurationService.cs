namespace CcbStack.Core.Configuration;

/// <summary>
/// Discovers all registered <see cref="ICcbStackConfigurationProvider"/> implementations,
/// invokes them in precedence order, merges their partial values, validates the merged
/// result, and returns an immutable effective configuration alongside structured
/// diagnostics and origin metadata.
/// </summary>
public interface ICcbStackConfigurationService
{
    ValueTask<CcbStackConfigurationResult> LoadAsync(
        CommandLineConfigurationInput commandLineInput,
        CancellationToken cancellationToken);
}
