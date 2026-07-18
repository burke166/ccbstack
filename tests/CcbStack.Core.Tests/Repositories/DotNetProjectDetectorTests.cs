using CcbStack.Core.IO;
using CcbStack.Core.Repositories;
using CcbStack.Core.Repositories.Model;
using CcbStack.Core.Tests.TestSupport;
using FluentAssertions;

namespace CcbStack.Core.Tests.Repositories;

public class DotNetProjectDetectorTests
{
    private readonly DotNetProjectDetector _detector = new(new FileSystem());

    [Fact]
    public async Task DetectAsync_ReturnsNotDetected_WhenNoProjectFilesExist()
    {
        var result = await _detector.DetectAsync(@"C:\repo", [], CancellationToken.None);

        result.IsDetected.Should().BeFalse();
    }

    [Fact]
    public async Task DetectAsync_ClassifiesDesktopApp_ForUseWpfProject()
    {
        using var temp = new TempDirectory();
        WriteProject(temp, "App.csproj", """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>WinExe</OutputType>
                <TargetFramework>net10.0-windows</TargetFramework>
                <UseWPF>true</UseWPF>
              </PropertyGroup>
            </Project>
            """);

        var result = await Detect(temp, "App.csproj");

        result.Projects.Single().Kind.Should().Be(DotNetProjectKind.Desktop);
        result.TargetFrameworks.Should().Contain("net10.0-windows");
    }

    [Fact]
    public async Task DetectAsync_ClassifiesTest_ForXunitPackageReference()
    {
        using var temp = new TempDirectory();
        WriteProject(temp, "App.Tests.csproj", """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="xunit" Version="2.6.0" />
                <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
              </ItemGroup>
            </Project>
            """);

        var result = await Detect(temp, "App.Tests.csproj");

        result.Projects.Single().Kind.Should().Be(DotNetProjectKind.Test);
    }

    [Fact]
    public async Task DetectAsync_ClassifiesAspNetCoreWebApp_ForWebSdkWithController()
    {
        using var temp = new TempDirectory();
        Directory.CreateDirectory(temp.Combine("Controllers"));
        File.WriteAllText(temp.Combine("Controllers", "HomeController.cs"), "public class HomeController {}");
        WriteProject(temp, "Web.csproj", """
            <Project Sdk="Microsoft.NET.Sdk.Web">
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
              </PropertyGroup>
            </Project>
            """);

        var result = await Detect(temp, "Web.csproj", "Controllers/HomeController.cs");

        result.Projects.Single(p => p.Path == "Web.csproj").Kind.Should().Be(DotNetProjectKind.AspNetCoreWebApp);
    }

    [Fact]
    public async Task DetectAsync_ClassifiesMinimalApi_ForWebSdkWithoutControllers()
    {
        using var temp = new TempDirectory();
        WriteProject(temp, "Api.csproj", """
            <Project Sdk="Microsoft.NET.Sdk.Web">
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
              </PropertyGroup>
            </Project>
            """);

        var result = await Detect(temp, "Api.csproj");

        result.Projects.Single().Kind.Should().Be(DotNetProjectKind.MinimalApi);
    }

    [Fact]
    public async Task DetectAsync_ClassifiesBlazor_ForAspNetCoreComponentsPackage()
    {
        using var temp = new TempDirectory();
        WriteProject(temp, "BlazorApp.csproj", """
            <Project Sdk="Microsoft.NET.Sdk.Web">
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="8.0.0" />
              </ItemGroup>
            </Project>
            """);

        var result = await Detect(temp, "BlazorApp.csproj");

        result.Projects.Single().Kind.Should().Be(DotNetProjectKind.Blazor);
    }

    [Fact]
    public async Task DetectAsync_ClassifiesWorker_ForWorkerSdk()
    {
        using var temp = new TempDirectory();
        WriteProject(temp, "Worker.csproj", """
            <Project Sdk="Microsoft.NET.Sdk.Worker">
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
              </PropertyGroup>
            </Project>
            """);

        var result = await Detect(temp, "Worker.csproj");

        result.Projects.Single().Kind.Should().Be(DotNetProjectKind.Worker);
    }

    [Fact]
    public async Task DetectAsync_ClassifiesCli_ForSpectreConsoleCliPackageReference()
    {
        using var temp = new TempDirectory();
        WriteProject(temp, "Tool.csproj", """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net10.0</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="Spectre.Console.Cli" Version="0.49.0" />
              </ItemGroup>
            </Project>
            """);

        var result = await Detect(temp, "Tool.csproj");

        result.Projects.Single().Kind.Should().Be(DotNetProjectKind.Cli);
    }

    [Fact]
    public async Task DetectAsync_ClassifiesConsole_ForPlainExeOutputType()
    {
        using var temp = new TempDirectory();
        WriteProject(temp, "Tool.csproj", """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net10.0</TargetFramework>
              </PropertyGroup>
            </Project>
            """);

        var result = await Detect(temp, "Tool.csproj");

        result.Projects.Single().Kind.Should().Be(DotNetProjectKind.Console);
    }

    [Fact]
    public async Task DetectAsync_ClassifiesLibrary_ByDefault()
    {
        using var temp = new TempDirectory();
        WriteProject(temp, "Lib.csproj", """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
              </PropertyGroup>
            </Project>
            """);

        var result = await Detect(temp, "Lib.csproj");

        result.Projects.Single().Kind.Should().Be(DotNetProjectKind.Library);
    }

    [Fact]
    public async Task DetectAsync_HandlesMultiTargeting()
    {
        using var temp = new TempDirectory();
        WriteProject(temp, "Multi.csproj", """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFrameworks>net10.0;net8.0</TargetFrameworks>
              </PropertyGroup>
            </Project>
            """);

        var result = await Detect(temp, "Multi.csproj");

        result.Projects.Single().TargetFrameworks.Should().BeEquivalentTo(["net10.0", "net8.0"]);
    }

    [Fact]
    public async Task DetectAsync_ReturnsUnknownKind_ForMalformedXml_WithoutThrowing()
    {
        using var temp = new TempDirectory();
        File.WriteAllText(temp.Combine("Broken.csproj"), "<Project><Unclosed>");

        var result = await Detect(temp, "Broken.csproj");

        result.Projects.Single().Kind.Should().Be(DotNetProjectKind.Unknown);
    }

    [Fact]
    public async Task DetectAsync_DetectsSolutionFilesAndSupportFiles()
    {
        using var temp = new TempDirectory();
        File.WriteAllText(temp.Combine("App.sln"), "solution");
        File.WriteAllText(temp.Combine("global.json"), "{}");
        File.WriteAllText(temp.Combine("Directory.Build.props"), "<Project/>");
        File.WriteAllText(temp.Combine("Directory.Packages.props"), "<Project/>");

        var result = await _detector.DetectAsync(
            temp.Path, ["App.sln", "global.json", "Directory.Build.props", "Directory.Packages.props"], CancellationToken.None);

        result.SolutionFiles.Should().ContainSingle().Which.Should().Be("App.sln");
        result.HasGlobalJson.Should().BeTrue();
        result.HasDirectoryBuildProps.Should().BeTrue();
        result.HasDirectoryPackagesProps.Should().BeTrue();
    }

    private static void WriteProject(TempDirectory temp, string fileName, string contents)
    {
        File.WriteAllText(temp.Combine(fileName), contents);
    }

    private async Task<DotNetInfo> Detect(TempDirectory temp, params string[] relativeFilePaths)
    {
        return await _detector.DetectAsync(temp.Path, relativeFilePaths, CancellationToken.None);
    }
}
