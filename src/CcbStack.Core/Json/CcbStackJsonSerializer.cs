using System.Text.Json;
using System.Text.Json.Serialization;

namespace CcbStack.Core.Json;

/// <summary>
/// The shared <see cref="ICcbStackJsonSerializer"/> implementation. Options are created once
/// and reused, so every caller gets identical camelCase naming, case-insensitive matching,
/// string-based enum serialization, and <c>OptionalValue&lt;T&gt;</c> handling.
/// </summary>
public sealed class CcbStackJsonSerializer : ICcbStackJsonSerializer
{
    private static readonly JsonSerializerOptions CompactOptions = CreateOptions(indented: false);
    private static readonly JsonSerializerOptions IndentedOptions = CreateOptions(indented: true);

    public T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, CompactOptions);
    }

    public string Serialize<T>(T value, bool indented)
    {
        return JsonSerializer.Serialize(value, indented ? IndentedOptions : CompactOptions);
    }

    private static JsonSerializerOptions CreateOptions(bool indented)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = indented,
            // Omits OptionalValue<T> properties left at Unset (the CLR default for the
            // struct) from serialized output, while still writing explicit nulls and values.
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        };

        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.Converters.Add(new OptionalValueJsonConverterFactory());

        return options;
    }
}
