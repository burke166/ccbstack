namespace CcbStack.Core.Json;

/// <summary>
/// A small, centralized JSON serialization abstraction. Every part of ccbstack that reads
/// or writes JSON should go through this interface rather than creating its own
/// <c>JsonSerializerOptions</c>, so camelCase naming, enum handling, and
/// <c>OptionalValue&lt;T&gt;</c> presence semantics stay consistent everywhere.
/// </summary>
public interface ICcbStackJsonSerializer
{
    T? Deserialize<T>(string json);

    string Serialize<T>(T value, bool indented);
}
