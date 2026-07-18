using CcbStack.Core.Configuration;

namespace CcbStack.Cli.Commands;

/// <summary>
/// Bundles the effective configuration with resolved-executable runtime information for
/// display. Assembled by <see cref="ConfigCommand"/> — runtime executable resolution is not
/// part of configuration loading, so it does not belong in <see cref="CcbStackConfiguration"/>.
/// </summary>
/// <param name="Configuration">The effective ccbstack configuration.</param>
/// <param name="Runtime">Resolved runtime executable paths, for display only.</param>
public sealed record ConfigView(CcbStackConfiguration Configuration, RuntimeEnvironmentView Runtime);

/// <summary>Resolved (or unresolved) paths for external executables ccbstack integrates with.</summary>
public sealed record RuntimeEnvironmentView(string? PowerShellPath, string? GitPath, string? ClaudePath);
