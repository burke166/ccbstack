using CcbStack.Core.Configuration;
using CcbStack.Core.Json;
using FluentAssertions;

namespace CcbStack.Core.Tests.Json;

public class CcbStackJsonSerializerTests
{
    private readonly CcbStackJsonSerializer _serializer = new();

    [Fact]
    public void Serialize_UsesCamelCasePropertyNames()
    {
        var diagnostic = new ConfigurationDiagnostic("CFG001", ConfigurationDiagnosticSeverity.Error, "Bad value");

        var json = _serializer.Serialize(diagnostic, indented: false);

        json.Should().Contain("\"code\"");
        json.Should().Contain("\"severity\"");
        json.Should().Contain("\"message\"");
        json.Should().NotContain("\"Code\"");
    }

    [Fact]
    public void Serialize_WritesEnumsAsCamelCaseStrings()
    {
        var diagnostic = new ConfigurationDiagnostic("CFG001", ConfigurationDiagnosticSeverity.Error, "Bad value");

        var json = _serializer.Serialize(diagnostic, indented: false);

        json.Should().Contain("\"severity\":\"error\"");
    }

    [Fact]
    public void Deserialize_IsCasePropertyInsensitive()
    {
        const string json = """{ "CODE": "CFG001", "SEVERITY": "warning", "MESSAGE": "hi" }""";

        var diagnostic = _serializer.Deserialize<ConfigurationDiagnostic>(json);

        diagnostic.Should().NotBeNull();
        diagnostic!.Code.Should().Be("CFG001");
        diagnostic.Severity.Should().Be(ConfigurationDiagnosticSeverity.Warning);
    }

    [Fact]
    public void Serialize_Indented_ProducesMultilineOutput()
    {
        var diagnostic = new ConfigurationDiagnostic("CFG001", ConfigurationDiagnosticSeverity.Error, "Bad value");

        var indented = _serializer.Serialize(diagnostic, indented: true);
        var compact = _serializer.Serialize(diagnostic, indented: false);

        indented.Should().Contain(Environment.NewLine);
        compact.Should().NotContain(Environment.NewLine);
    }

    [Fact]
    public void Serialize_OmitsUnsetOptionalValueProperties()
    {
        var values = new CcbStackConfigurationValues
        {
            DefaultModel = OptionalValue<string>.Of("sonnet"),
        };

        var json = _serializer.Serialize(values, indented: false);

        json.Should().Contain("\"defaultModel\":\"sonnet\"");
        json.Should().NotContain("skillsDirectory");
        json.Should().NotContain("outputFormat");
    }

    [Fact]
    public void Serialize_WritesExplicitNullOptionalValueAsJsonNull()
    {
        var values = new CcbStackConfigurationValues
        {
            DefaultModel = OptionalValue<string>.Null(),
        };

        var json = _serializer.Serialize(values, indented: false);

        json.Should().Contain("\"defaultModel\":null");
    }

    [Fact]
    public void Deserialize_AbsentPropertyLeavesOptionalValueUnset()
    {
        const string json = """{ "defaultModel": "sonnet" }""";

        var values = _serializer.Deserialize<CcbStackConfigurationValues>(json);

        values.Should().NotBeNull();
        values!.DefaultModel.Should().Be(OptionalValue<string>.Of("sonnet"));
        values.SkillsDirectory.Should().Be(OptionalValue<string>.Unset);
    }

    [Fact]
    public void Deserialize_ExplicitJsonNullMapsToOptionalValueNull()
    {
        const string json = """{ "defaultModel": null }""";

        var values = _serializer.Deserialize<CcbStackConfigurationValues>(json);

        values.Should().NotBeNull();
        values!.DefaultModel.IsSet.Should().BeTrue();
        values.DefaultModel.IsNull.Should().BeTrue();
    }

    [Fact]
    public void Deserialize_ExplicitValueMapsToOptionalValueOf()
    {
        const string json = """{ "defaultModel": "haiku" }""";

        var values = _serializer.Deserialize<CcbStackConfigurationValues>(json);

        values.Should().NotBeNull();
        values!.DefaultModel.Should().Be(OptionalValue<string>.Of("haiku"));
    }
}
