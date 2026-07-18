using CcbStack.Core.IO;

namespace CcbStack.Core.Tests.TestSupport;

public sealed class FakeEnvironmentVariableReader : IEnvironmentVariableReader
{
    private readonly Dictionary<string, string?> _variables;

    public FakeEnvironmentVariableReader(IDictionary<string, string?>? variables = null)
    {
        _variables = new Dictionary<string, string?>(
            variables ?? new Dictionary<string, string?>(),
            StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyDictionary<string, string?> GetVariables() => _variables;
}
