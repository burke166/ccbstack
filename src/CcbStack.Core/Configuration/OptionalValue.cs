namespace CcbStack.Core.Configuration;

/// <summary>
/// Wraps a configuration property value while distinguishing three states: not supplied,
/// explicitly supplied as <see langword="null"/>, and explicitly supplied with a value
/// (including "empty-looking" values such as <c>""</c>, <see langword="false"/>, or
/// <c>0</c>). Presence is tracked independently of <typeparamref name="T"/>, so callers
/// never need to reason about CLR default values to determine whether a source supplied
/// a value.
/// </summary>
public readonly struct OptionalValue<T> : IEquatable<OptionalValue<T>>
{
    private OptionalValue(bool isSet, bool isNull, T? value)
    {
        IsSet = isSet;
        IsNull = isNull;
        Value = value;
    }

    /// <summary>
    /// <see langword="true"/> when a source explicitly supplied this property, whether the
    /// supplied value was <see langword="null"/> or not.
    /// </summary>
    public bool IsSet { get; }

    /// <summary>
    /// <see langword="true"/> when the property was supplied and explicitly set to
    /// <see langword="null"/>. Only meaningful when <see cref="IsSet"/> is <see langword="true"/>.
    /// </summary>
    public bool IsNull { get; }

    /// <summary>The supplied value, or <see langword="default"/> when not set or explicitly null.</summary>
    public T? Value { get; }

    /// <summary>The "not supplied" state. Equivalent to <see langword="default"/>.</summary>
    public static OptionalValue<T> Unset => default;

    /// <summary>Creates a value explicitly supplied as <paramref name="value"/>.</summary>
    public static OptionalValue<T> Of(T value) => new(isSet: true, isNull: false, value);

    /// <summary>Creates a value explicitly supplied as <see langword="null"/>.</summary>
    public static OptionalValue<T> Null() => new(isSet: true, isNull: true, value: default);

    public bool Equals(OptionalValue<T> other) =>
        IsSet == other.IsSet &&
        IsNull == other.IsNull &&
        EqualityComparer<T?>.Default.Equals(Value, other.Value);

    public override bool Equals(object? obj) => obj is OptionalValue<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(IsSet, IsNull, Value);

    public static bool operator ==(OptionalValue<T> left, OptionalValue<T> right) => left.Equals(right);

    public static bool operator !=(OptionalValue<T> left, OptionalValue<T> right) => !left.Equals(right);
}
