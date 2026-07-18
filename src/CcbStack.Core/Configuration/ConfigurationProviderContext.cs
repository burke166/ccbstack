namespace CcbStack.Core.Configuration;

/// <summary>
/// Runtime inputs made available to every <see cref="ICcbStackConfigurationProvider"/>
/// invocation, so providers do not read global process state directly.
/// </summary>
/// <param name="CurrentDirectory">The current working directory to resolve relative paths against.</param>
/// <param name="CommandLineInput">The raw command-line configuration overrides, if any.</param>
public sealed record ConfigurationProviderContext(
    string CurrentDirectory,
    CommandLineConfigurationInput CommandLineInput);
