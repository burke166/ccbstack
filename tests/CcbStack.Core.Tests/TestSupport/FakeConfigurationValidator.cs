using CcbStack.Core.Configuration;

namespace CcbStack.Core.Tests.TestSupport;

public sealed class FakeConfigurationValidator : ICcbStackConfigurationValidator
{
    private readonly IReadOnlyList<ConfigurationDiagnostic> _diagnostics;

    public FakeConfigurationValidator(params ConfigurationDiagnostic[] diagnostics)
    {
        _diagnostics = diagnostics;
    }

    public CcbStackConfiguration? LastValidated { get; private set; }

    public IReadOnlyList<ConfigurationDiagnostic> Validate(CcbStackConfiguration configuration)
    {
        LastValidated = configuration;
        return _diagnostics;
    }
}
