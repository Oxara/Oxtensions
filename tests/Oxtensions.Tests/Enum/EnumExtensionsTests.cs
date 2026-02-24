// -----------------------------------------------------------------------
// <copyright file="EnumExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Oxtensions.Enum;
using Xunit;

namespace Oxtensions.Tests.Enum;

// ── Test enum fixtures ────────────────────────────────────────────────────────

public enum TestStatus
{
    [Description("Active User")]
    [Display(Name = "Active")]
    Active = 1,

    [Description("Inactive User")]
    [Display(Name = "Inactive")]
    Inactive = 2,

    NoAttributes = 3
}

[Flags]
public enum TestPermission
{
    None = 0,
    Read = 1,
    Write = 2,
    Execute = 4
}

// ── Tests ────────────────────────────────────────────────────────────────────

public sealed class EnumExtensionsTests
{
    // ── GetDescription ────────────────────────────────────────────────────────

    [Fact]
    public void GetDescription_WithAttribute_ReturnsDescription()
        => TestStatus.Active.GetDescription().Should().Be("Active User");

    [Fact]
    public void GetDescription_NoAttribute_ReturnsMemberName()
        => TestStatus.NoAttributes.GetDescription().Should().Be("NoAttributes");

    [Theory]
    [InlineData(TestStatus.Active, "Active User")]
    [InlineData(TestStatus.Inactive, "Inactive User")]
    [InlineData(TestStatus.NoAttributes, "NoAttributes")]
    public void GetDescription_VariousValues_ReturnsExpected(TestStatus status, string expected)
        => status.GetDescription().Should().Be(expected);

    // ── GetDisplayName ────────────────────────────────────────────────────────

    [Fact]
    public void GetDisplayName_WithAttribute_ReturnsDisplayName()
        => TestStatus.Active.GetDisplayName().Should().Be("Active");

    [Fact]
    public void GetDisplayName_NoAttribute_ReturnsMemberName()
        => TestStatus.NoAttributes.GetDisplayName().Should().Be("NoAttributes");

    // ── ToList ────────────────────────────────────────────────────────────────

    [Fact]
    public void ToList_EnumType_ReturnsAllValues()
    {
        var list = EnumExtensions.ToList<TestStatus>();
        list.Should().HaveCount(3);
        list.Should().Contain(TestStatus.Active);
        list.Should().Contain(TestStatus.Inactive);
        list.Should().Contain(TestStatus.NoAttributes);
    }

    [Fact]
    public void ToList_DayOfWeek_Returns7Values()
        => EnumExtensions.ToList<DayOfWeek>().Should().HaveCount(7);

    // ── HasFlag ───────────────────────────────────────────────────────────────

    [Fact]
    public void HasFlag_FlagSet_ReturnsTrue()
        => (TestPermission.Read | TestPermission.Write).HasFlag(TestPermission.Read).Should().BeTrue();

    [Fact]
    public void HasFlag_FlagNotSet_ReturnsFalse()
        => TestPermission.Read.HasFlag(TestPermission.Write).Should().BeFalse();

    [Theory]
    [InlineData(TestPermission.Read | TestPermission.Write, TestPermission.Read, true)]
    [InlineData(TestPermission.Read | TestPermission.Write, TestPermission.Execute, false)]
    public void HasFlag_VariousFlags_ReturnsExpected(TestPermission value, TestPermission flag, bool expected)
        => value.HasFlag(flag).Should().Be(expected);

    // ── Parse ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Parse_ValidString_ReturnsEnumValue()
        => EnumExtensions.Parse<TestStatus>("Active").Should().Be(TestStatus.Active);

    [Fact]
    public void Parse_CaseInsensitive_ReturnsEnumValue()
        => EnumExtensions.Parse<TestStatus>("active").Should().Be(TestStatus.Active);

    [Fact]
    public void Parse_InvalidString_ReturnsDefault()
        => EnumExtensions.Parse<TestStatus>("Unknown").Should().Be(default(TestStatus));

    // ── IsValid ───────────────────────────────────────────────────────────────

    [Fact]
    public void IsValid_DefinedValue_ReturnsTrue()
        => EnumExtensions.IsValid<TestStatus>(1).Should().BeTrue();

    [Fact]
    public void IsValid_UndefinedValue_ReturnsFalse()
        => EnumExtensions.IsValid<TestStatus>(999).Should().BeFalse();

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    public void IsValid_VariousValues_ReturnsExpected(int value, bool expected)
        => EnumExtensions.IsValid<TestStatus>(value).Should().Be(expected);
}
