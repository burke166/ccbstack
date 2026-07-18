namespace CcbStack.Core.Configuration;

/// <summary>
/// Contributes configuration values from a single source (defaults, a file, environment
/// variables, or command-line overrides). Implementations must only read, parse, and report
/// diagnostics for their own source — they must not validate the final merged configuration.
/// Register additional implementations through dependency injection to extend the
/// configuration system without modifying <see cref="ICcbStackConfigurationService"/>.
/// </summary>
public interface ICcbStackConfigurationProvider
{
    /// <summary>The precedence layer this provider's values belong to.</summary>
    ConfigurationLayer Layer { get; }

    /// <summary>Reads and parses this provider's source, returning its partial configuration values.</summary>
    ValueTask<CcbStackConfigurationSource> LoadAsync(
        ConfigurationProviderContext context,
        CancellationToken cancellationToken);
}
