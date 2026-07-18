using CcbStack.Core.Repositories;
using CcbStack.Core.Repositories.Model;
using FluentAssertions;

namespace CcbStack.Core.Tests.Repositories;

public class LanguageDetectorTests
{
    private readonly LanguageDetector _detector = new();

    [Fact]
    public void Detect_ReturnsEmpty_WhenNoRecognizedExtensionsExist()
    {
        var result = _detector.Detect(["README.md", "LICENSE"]);

        result.Should().BeEmpty();
    }

    [Fact]
    public void Detect_ReturnsDistinctLanguages_InDeclarationOrder()
    {
        var result = _detector.Detect(["b.ts", "a.cs", "c.cs", "d.go", "e.ps1"]);

        result.Should().Equal(
            RepositoryLanguage.CSharp,
            RepositoryLanguage.Go,
            RepositoryLanguage.PowerShell,
            RepositoryLanguage.TypeScript);
    }

    [Theory]
    [InlineData("Program.cs", RepositoryLanguage.CSharp)]
    [InlineData("Program.fs", RepositoryLanguage.FSharp)]
    [InlineData("Program.vb", RepositoryLanguage.VisualBasic)]
    [InlineData("main.go", RepositoryLanguage.Go)]
    [InlineData("script.ps1", RepositoryLanguage.PowerShell)]
    [InlineData("Module.psm1", RepositoryLanguage.PowerShell)]
    [InlineData("index.js", RepositoryLanguage.JavaScript)]
    [InlineData("component.jsx", RepositoryLanguage.JavaScript)]
    [InlineData("index.ts", RepositoryLanguage.TypeScript)]
    [InlineData("component.tsx", RepositoryLanguage.TypeScript)]
    public void Detect_MapsExtensionToLanguage(string fileName, RepositoryLanguage expected)
    {
        var result = _detector.Detect([fileName]);

        result.Should().ContainSingle().Which.Should().Be(expected);
    }
}
