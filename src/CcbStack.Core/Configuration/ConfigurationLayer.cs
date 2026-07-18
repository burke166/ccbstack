namespace CcbStack.Core.Configuration;

/// <summary>
/// The precedence layer a configuration source belongs to. Higher numeric values override
/// lower ones during merge; application code should refer to the named members rather than
/// the underlying numbers.
/// </summary>
public enum ConfigurationLayer
{
    Defaults = 0,
    User = 100,
    Project = 200,
    Environment = 300,
    CommandLine = 400,
}
