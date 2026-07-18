namespace CcbStack.Cli;

/// <summary>
/// Centralized process exit codes, per CLAUDE.md's exit-code convention. Commands should
/// return these constants rather than magic numbers.
/// </summary>
public static class CcbStackExitCodes
{
    public const int Success = 0;
    public const int CheckFailed = 1;
    public const int InvalidCommandLine = 2;
    public const int InvalidConfiguration = 3;
    public const int UnsupportedRepository = 4;
    public const int DependencyUnavailable = 5;
    public const int SafetyPolicyBlocked = 6;
    public const int UnexpectedError = 7;
}
