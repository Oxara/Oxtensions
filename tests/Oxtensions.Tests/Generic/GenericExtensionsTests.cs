// -----------------------------------------------------------------------
// <copyright file="GenericExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Generic;
using Xunit;

namespace Oxtensions.Tests.Generic;

// ── Test fixture ──────────────────────────────────────────────────────────────

public sealed class PersonDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<string> Tags { get; set; } = [];
}

// ── Tests ─────────────────────────────────────────────────────────────────────

public sealed class GenericExtensionsTests
{
    // ── IsNull / IsNotNull ────────────────────────────────────────────────────

    [Fact]
    public void IsNull_NullReference_ReturnsTrue()
        => ((string?)null).IsNull().Should().BeTrue();

    [Fact]
    public void IsNull_NonNullValue_ReturnsFalse()
        => "hello".IsNull().Should().BeFalse();

    [Fact]
    public void IsNotNull_NonNull_ReturnsTrue()
        => 42.IsNotNull().Should().BeTrue();

    [Fact]
    public void IsNotNull_Null_ReturnsFalse()
        => ((object?)null).IsNotNull().Should().BeFalse();

    // ── ThrowIfNull ────────────────────────────────────────────────────────────

    [Fact]
    public void ThrowIfNull_NonNull_ReturnsObject()
    {
        const string value = "hello";
        value.ThrowIfNull(nameof(value)).Should().Be("hello");
    }

    [Fact]
    public void ThrowIfNull_Null_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value.ThrowIfNull(nameof(value));
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ThrowIfNull_ExceptionContainsParamName()
    {
        string? x = null;
        Action act = () => x.ThrowIfNull("myParam");
        act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("myParam");
    }

    // ── In ─────────────────────────────────────────────────────────────────────

    [Fact]
    public void In_ValueInSet_ReturnsTrue()
        => 5.In(1, 3, 5, 7).Should().BeTrue();

    [Fact]
    public void In_ValueNotInSet_ReturnsFalse()
        => 9.In(1, 3, 5, 7).Should().BeFalse();

    [Fact]
    public void In_StringMatch_ReturnsTrue()
        => "b".In("a", "b", "c").Should().BeTrue();

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, false)]
    [InlineData(3, true)]
    public void In_VariousIntegers_ReturnsExpected(int value, bool expected)
        => value.In(1, 3, 5).Should().Be(expected);

    // ── Clone ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Clone_Object_ReturnsDeepCopy()
    {
        var original = new PersonDto { Id = 1, Name = "Alice", Tags = ["dev", "csharp"] };
        var clone = original.Clone();

        clone.Should().NotBeSameAs(original);
        clone!.Id.Should().Be(1);
        clone.Name.Should().Be("Alice");

        // Mutate clone — must not affect original
        clone.Tags.Add("dotnet");
        original.Tags.Should().HaveCount(2);
    }

    [Fact]
    public void Clone_NullObject_ReturnsNull()
        => ((PersonDto?)null).Clone().Should().BeNull();

    // ── ToJson / FromJson ──────────────────────────────────────────────────────

    [Fact]
    public void ToJson_Object_ReturnsJsonString()
    {
        var dto = new PersonDto { Id = 42, Name = "Bob" };
        string json = dto.ToJson();
        json.Should().Contain("42");
        json.Should().Contain("Bob");
    }

    [Fact]
    public void FromJson_ValidJson_ReturnsDeserialized()
    {
        string json = "{\"id\":7,\"name\":\"Eve\"}";
        var dto = json.FromJson<PersonDto>();
        dto!.Id.Should().Be(7);
        dto.Name.Should().Be("Eve");
    }

    [Fact]
    public void FromJson_InvalidJson_ReturnsDefault()
        => "not json".FromJson<PersonDto>().Should().BeNull();

    [Fact]
    public void FromJson_NullString_ReturnsDefault()
        => ((string?)null).FromJson<PersonDto>().Should().BeNull();

    [Fact]
    public void ToJson_ThenFromJson_RoundTrips()
    {
        var original = new PersonDto { Id = 1, Name = "Test", Tags = ["a", "b"] };
        var restored = original.ToJson().FromJson<PersonDto>();
        restored!.Id.Should().Be(original.Id);
        restored.Name.Should().Be(original.Name);
        restored.Tags.Should().Equal(original.Tags);
    }
}

public sealed class GenericExtensions_PipeTests
{
    [Fact]
    public void Pipe_TransformsValue()
        => "hello".Pipe(s => s.Length).Should().Be(5);

    [Fact]
    public void Pipe_NullFunc_ThrowsArgumentNullException()
    {
        var act = () => "x".Pipe<string, int>(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Pipe_CanChainMultipleTimes()
        => 5.Pipe(x => x * 2).Pipe(x => x + 1).Should().Be(11);
}

public sealed class GenericExtensions_AlsoTests
{
    [Fact]
    public void Also_ExecutesSideEffect_ReturnsOriginal()
    {
        int sideEffect = 0;
        var result = 42.Also(x => sideEffect = x);
        result.Should().Be(42);
        sideEffect.Should().Be(42);
    }

    [Fact]
    public void Also_NullAction_ThrowsArgumentNullException()
    {
        var act = () => "x".Also(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}

public sealed class GenericExtensions_IfNotNullTests
{
    [Fact]
    public void IfNotNull_NonNull_ExecutesAction()
    {
        int executed = 0;
        "hello".IfNotNull(_ => executed++);
        executed.Should().Be(1);
    }

    [Fact]
    public void IfNotNull_Null_DoesNotExecuteAction()
    {
        int executed = 0;
        ((string?)null).IfNotNull(_ => executed++);
        executed.Should().Be(0);
    }

    [Fact]
    public void IfNotNull_ReturnsOriginalValue()
        => "hello".IfNotNull(_ => { }).Should().Be("hello");
}
