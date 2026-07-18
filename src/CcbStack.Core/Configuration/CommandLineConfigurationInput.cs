namespace CcbStack.Core.Configuration;

/// <summary>
/// Temporary configuration overrides supplied on the command line. Never persisted.
/// </summary>
/// <param name="Json">An inline JSON object supplied via <c>--config-json</c>, when present.</param>
/// <param name="FilePath">A path to a JSON file supplied via <c>--config-file</c>, when present.</param>
public sealed record CommandLineConfigurationInput(string? Json, string? FilePath);
