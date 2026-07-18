using System.Text.Json;
using CcbStack.Core.IO;
using CcbStack.Core.Json;
using CcbStack.Core.Runtime;

namespace CcbStack.Core.Configuration.Providers;

/// <summary>
/// Shared JSON config-file reading and parsing logic used by
/// <see cref="UserConfigurationProvider"/>, <see cref="ProjectConfigurationProvider"/>, and
/// <see cref="CommandLineConfigurationProvider"/>. Not a provider itself — each caller owns
/// its own file-path resolution and missing-file semantics (missing is normal for
/// user/project; an error for an explicit <c>--config-file</c>).
/// </summary>
internal static class ConfigFileLoader
{
    private static readonly string[] KnownTopLevelKeys = ["defaultModel", "skillsDirectory", "output"];
    private static readonly string[] KnownOutputKeys = ["format"];

    /// <summary>Reads and parses <paramref name="filePath"/>, or returns an empty source if it does not exist.</summary>
    public static async ValueTask<CcbStackConfigurationSource> LoadOptionalFileAsync(
        string providerName,
        ConfigurationLayer layer,
        string filePath,
        IFileSystem fileSystem,
        ICcbStackJsonSerializer jsonSerializer,
        ConfigurationPathExpander pathExpander,
        IRuntimeEnvironment runtimeEnvironment,
        CancellationToken cancellationToken)
    {
        if (!fileSystem.FileExists(filePath))
        {
            return EmptySource(providerName, layer);
        }

        return await ReadAndParseAsync(
            providerName, layer, filePath, fileSystem, jsonSerializer, pathExpander, runtimeEnvironment, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>Reads and parses <paramref name="filePath"/>, assuming it already exists.</summary>
    public static async ValueTask<CcbStackConfigurationSource> ReadAndParseAsync(
        string providerName,
        ConfigurationLayer layer,
        string filePath,
        IFileSystem fileSystem,
        ICcbStackJsonSerializer jsonSerializer,
        ConfigurationPathExpander pathExpander,
        IRuntimeEnvironment runtimeEnvironment,
        CancellationToken cancellationToken)
    {
        string json;

        try
        {
            json = await fileSystem.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return ErrorSource(
                providerName,
                layer,
                "CFG002",
                $"Could not read configuration file '{filePath}': {ex.Message}",
                filePath);
        }

        return ParseJson(providerName, layer, filePath, json, jsonSerializer, pathExpander, runtimeEnvironment);
    }

    /// <summary>
    /// Parses <paramref name="json"/> into partial configuration values. <paramref name="sourcePath"/>
    /// is <see langword="null"/> for inline <c>--config-json</c> input, in which case relative
    /// paths resolve against <see cref="IRuntimeEnvironment.CurrentDirectory"/> instead of a
    /// containing file's directory.
    /// </summary>
    public static CcbStackConfigurationSource ParseJson(
        string providerName,
        ConfigurationLayer layer,
        string? sourcePath,
        string json,
        ICcbStackJsonSerializer jsonSerializer,
        ConfigurationPathExpander pathExpander,
        IRuntimeEnvironment runtimeEnvironment)
    {
        List<ConfigurationDiagnostic> diagnostics;

        try
        {
            diagnostics = DetectUnknownProperties(json, sourcePath, providerName);
        }
        catch (JsonException ex)
        {
            return ErrorSource(providerName, layer, "CFG001", InvalidJsonMessage(sourcePath, ex), sourcePath);
        }

        ConfigFileContract? contract;

        try
        {
            contract = jsonSerializer.Deserialize<ConfigFileContract>(json);
        }
        catch (JsonException ex)
        {
            return ErrorSource(providerName, layer, "CFG001", InvalidJsonMessage(sourcePath, ex), sourcePath);
        }

        if (contract is null)
        {
            return EmptySource(providerName, layer);
        }

        var baseDirectory = sourcePath is not null
            ? Path.GetDirectoryName(Path.GetFullPath(sourcePath)) ?? runtimeEnvironment.CurrentDirectory
            : runtimeEnvironment.CurrentDirectory;

        var values = new CcbStackConfigurationValues
        {
            DefaultModel = contract.DefaultModel,
            SkillsDirectory = ExpandIfPresent(contract.SkillsDirectory, baseDirectory, pathExpander, runtimeEnvironment),
            OutputFormat = contract.Output?.Format ?? OptionalValue<string>.Unset,
        };

        var origins = BuildOrigins(providerName, layer, sourcePath, values);

        return new CcbStackConfigurationSource(providerName, layer, values, diagnostics)
        {
            Origins = origins,
        };
    }

    public static CcbStackConfigurationSource EmptySource(string providerName, ConfigurationLayer layer)
    {
        return new CcbStackConfigurationSource(providerName, layer, new CcbStackConfigurationValues(), []);
    }

    public static CcbStackConfigurationSource ErrorSource(
        string providerName,
        ConfigurationLayer layer,
        string code,
        string message,
        string? sourcePath)
    {
        var diagnostic = new ConfigurationDiagnostic(code, ConfigurationDiagnosticSeverity.Error, message, sourcePath ?? providerName);
        return new CcbStackConfigurationSource(providerName, layer, new CcbStackConfigurationValues(), [diagnostic]);
    }

    private static OptionalValue<string> ExpandIfPresent(
        OptionalValue<string> value,
        string baseDirectory,
        ConfigurationPathExpander pathExpander,
        IRuntimeEnvironment runtimeEnvironment)
    {
        if (!value.IsSet || value.IsNull || value.Value is null)
        {
            return value;
        }

        return OptionalValue<string>.Of(pathExpander.Expand(value.Value, baseDirectory, runtimeEnvironment));
    }

    private static Dictionary<string, ConfigurationValueOrigin> BuildOrigins(
        string providerName,
        ConfigurationLayer layer,
        string? sourcePath,
        CcbStackConfigurationValues values)
    {
        var origins = new Dictionary<string, ConfigurationValueOrigin>();

        AddOrigin(origins, values.DefaultModel, CcbStackConfigurationKeys.DefaultModel, providerName, layer, sourcePath);
        AddOrigin(origins, values.SkillsDirectory, CcbStackConfigurationKeys.SkillsDirectory, providerName, layer, sourcePath);
        AddOrigin(origins, values.OutputFormat, CcbStackConfigurationKeys.Output.Format, providerName, layer, sourcePath);

        return origins;
    }

    private static void AddOrigin(
        Dictionary<string, ConfigurationValueOrigin> origins,
        OptionalValue<string> value,
        string key,
        string providerName,
        ConfigurationLayer layer,
        string? sourcePath)
    {
        if (!value.IsSet)
        {
            return;
        }

        origins[key] = new ConfigurationValueOrigin(providerName, layer, key, SourcePath: sourcePath, JsonPropertyPath: key);
    }

    private static List<ConfigurationDiagnostic> DetectUnknownProperties(string json, string? sourcePath, string providerName)
    {
        var diagnostics = new List<ConfigurationDiagnostic>();

        using var document = JsonDocument.Parse(json);

        if (document.RootElement.ValueKind != JsonValueKind.Object)
        {
            return diagnostics;
        }

        foreach (var property in document.RootElement.EnumerateObject())
        {
            if (string.Equals(property.Name, "output", StringComparison.OrdinalIgnoreCase))
            {
                if (property.Value.ValueKind == JsonValueKind.Object)
                {
                    foreach (var nested in property.Value.EnumerateObject())
                    {
                        if (!KnownOutputKeys.Contains(nested.Name, StringComparer.OrdinalIgnoreCase))
                        {
                            diagnostics.Add(UnknownPropertyDiagnostic(
                                $"output.{nested.Name}",
                                KnownOutputKeys.Select(k => $"output.{k}"),
                                sourcePath,
                                providerName));
                        }
                    }
                }

                continue;
            }

            if (!KnownTopLevelKeys.Contains(property.Name, StringComparer.OrdinalIgnoreCase))
            {
                diagnostics.Add(UnknownPropertyDiagnostic(property.Name, KnownTopLevelKeys, sourcePath, providerName));
            }
        }

        return diagnostics;
    }

    private static ConfigurationDiagnostic UnknownPropertyDiagnostic(
        string propertyName,
        IEnumerable<string> knownKeys,
        string? sourcePath,
        string providerName)
    {
        var suggestion = ConfigurationKeySuggestions.FindClosestMatch(propertyName, knownKeys);
        var message = suggestion is null
            ? $"Unrecognized configuration property '{propertyName}'."
            : $"Unrecognized configuration property '{propertyName}'. Did you mean '{suggestion}'?";

        return new ConfigurationDiagnostic("CFG003", ConfigurationDiagnosticSeverity.Warning, message, sourcePath ?? providerName, propertyName);
    }

    private static string InvalidJsonMessage(string? sourcePath, JsonException ex)
    {
        return $"Configuration source '{sourcePath ?? "inline"}' contains invalid JSON: {ex.Message}";
    }
}
