using CcbStack.Core.Configuration;

namespace CcbStack.Cli.Tests.TestSupport;

public sealed class FakeCcbStackConfigurationService : ICcbStackConfigurationService
{
    private readonly Func<CommandLineConfigurationInput, CcbStackConfigurationResult> _load;

    private FakeCcbStackConfigurationService(Func<CommandLineConfigurationInput, CcbStackConfigurationResult> load)
    {
        _load = load;
    }

    public static FakeCcbStackConfigurationService Returning(CcbStackConfigurationResult result)
    {
        return new FakeCcbStackConfigurationService(_ => result);
    }

    public static FakeCcbStackConfigurationService Throwing(Exception exception)
    {
        return new FakeCcbStackConfigurationService(_ => throw exception);
    }

    public ValueTask<CcbStackConfigurationResult> LoadAsync(
        CommandLineConfigurationInput commandLineInput, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(_load(commandLineInput));
    }

    public static CcbStackConfigurationResult SuccessResult(
        string defaultModel = "sonnet", string skillsDirectory = @"C:\skills", string outputFormat = "text")
    {
        var configuration = new CcbStackConfiguration(defaultModel, skillsDirectory, new OutputConfiguration(outputFormat));

        return new CcbStackConfigurationResult(
            configuration,
            new Dictionary<string, ConfigurationValueOrigin>(),
            []);
    }
}
