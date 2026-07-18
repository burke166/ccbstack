using CcbStack.Core.Configuration;
using FluentAssertions;

namespace CcbStack.Core.Tests.Configuration;

public class OptionalValueTests
{
    [Fact]
    public void Unset_IsNotSet()
    {
        var value = OptionalValue<string>.Unset;

        value.IsSet.Should().BeFalse();
        value.IsNull.Should().BeFalse();
        value.Value.Should().BeNull();
    }

    [Fact]
    public void Default_IsEquivalentToUnset()
    {
        var value = default(OptionalValue<string>);

        value.Should().Be(OptionalValue<string>.Unset);
    }

    [Fact]
    public void Of_IsSetAndNotNull()
    {
        var value = OptionalValue<string>.Of("sonnet");

        value.IsSet.Should().BeTrue();
        value.IsNull.Should().BeFalse();
        value.Value.Should().Be("sonnet");
    }

    [Fact]
    public void Null_IsSetAndNull()
    {
        var value = OptionalValue<string>.Null();

        value.IsSet.Should().BeTrue();
        value.IsNull.Should().BeTrue();
        value.Value.Should().BeNull();
    }

    [Fact]
    public void Of_DistinguishesExplicitEmptyStringFromUnset()
    {
        var empty = OptionalValue<string>.Of(string.Empty);
        var unset = OptionalValue<string>.Unset;

        empty.IsSet.Should().BeTrue();
        empty.Value.Should().Be(string.Empty);
        empty.Should().NotBe(unset);
    }

    [Fact]
    public void Of_DistinguishesExplicitFalseFromUnset()
    {
        var explicitFalse = OptionalValue<bool>.Of(false);
        var unset = OptionalValue<bool>.Unset;

        explicitFalse.IsSet.Should().BeTrue();
        explicitFalse.Value.Should().BeFalse();
        explicitFalse.Should().NotBe(unset);
    }

    [Fact]
    public void Of_DistinguishesExplicitZeroFromUnset()
    {
        var explicitZero = OptionalValue<int>.Of(0);
        var unset = OptionalValue<int>.Unset;

        explicitZero.IsSet.Should().BeTrue();
        explicitZero.Value.Should().Be(0);
        explicitZero.Should().NotBe(unset);
    }

    [Fact]
    public void Equality_ComparesStateAndValue()
    {
        OptionalValue<string>.Of("sonnet").Should().Be(OptionalValue<string>.Of("sonnet"));
        OptionalValue<string>.Of("sonnet").Should().NotBe(OptionalValue<string>.Of("haiku"));
        OptionalValue<string>.Null().Should().NotBe(OptionalValue<string>.Unset);
    }
}
