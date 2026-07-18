using System.Text.Json;
using System.Text.Json.Serialization;
using CcbStack.Core.Configuration;

namespace CcbStack.Core.Json;

/// <summary>
/// Serializes and deserializes <see cref="OptionalValue{T}"/> so its three states round-trip
/// correctly through JSON: a JSON property that is absent leaves the target property at its
/// CLR default (<see cref="OptionalValue{T}.Unset"/>, since this converter is never invoked
/// for properties that do not appear in the payload); a JSON <see langword="null"/> maps to
/// <see cref="OptionalValue{T}.Null"/>; any other JSON value maps to
/// <see cref="OptionalValue{T}.Of"/>.
/// </summary>
public sealed class OptionalValueJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(OptionalValue<>);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var innerType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(OptionalValueJsonConverter<>).MakeGenericType(innerType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private sealed class OptionalValueJsonConverter<T> : JsonConverter<OptionalValue<T>>
    {
        public override OptionalValue<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return OptionalValue<T>.Null();
            }

            var value = JsonSerializer.Deserialize<T>(ref reader, options);
            return value is null ? OptionalValue<T>.Null() : OptionalValue<T>.Of(value);
        }

        public override void Write(Utf8JsonWriter writer, OptionalValue<T> value, JsonSerializerOptions options)
        {
            if (!value.IsSet || value.IsNull)
            {
                writer.WriteNullValue();
                return;
            }

            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}
