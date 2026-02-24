// -----------------------------------------------------------------------
// <copyright file="GuidExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using FluentAssertions;
using Oxtensions.Identifiers;
using Xunit;

public sealed class GuidExtensions_IsEmptyTests
{
    [Fact]
    public void IsEmpty_EmptyGuid_ReturnsTrue()
        => System.Guid.Empty.IsEmpty().Should().BeTrue();

    [Fact]
    public void IsEmpty_NewGuid_ReturnsFalse()
        => System.Guid.NewGuid().IsEmpty().Should().BeFalse();
}

public sealed class GuidExtensions_IsNullOrEmptyTests
{
    [Fact]
    public void IsNullOrEmpty_NullGuid_ReturnsTrue()
        => ((System.Guid?)null).IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_EmptyGuid_ReturnsTrue()
        => ((System.Guid?)System.Guid.Empty).IsNullOrEmpty().Should().BeTrue();

    [Fact]
    public void IsNullOrEmpty_ValidGuid_ReturnsFalse()
        => ((System.Guid?)System.Guid.NewGuid()).IsNullOrEmpty().Should().BeFalse();
}

public sealed class GuidExtensions_ToShortStringTests
{
    [Fact]
    public void ToShortString_Returns22CharString()
        => System.Guid.NewGuid().ToShortString().Should().HaveLength(22);

    [Fact]
    public void ToShortString_IsUrlSafe()
        => System.Guid.NewGuid().ToShortString().Should().MatchRegex("^[A-Za-z0-9_-]+$");

    [Fact]
    public void ToShortString_DifferentGuids_DifferentStrings()
    {
        var a = System.Guid.NewGuid().ToShortString();
        var b = System.Guid.NewGuid().ToShortString();
        a.Should().NotBe(b);
    }
}

public sealed class GuidExtensions_NewCombTests
{
    [Fact]
    public void NewComb_ReturnsNonEmptyGuid()
        => GuidExtensions.NewComb().Should().NotBe(System.Guid.Empty);

    [Fact]
    public void NewComb_CalledTwice_ReturnsDistinctValues()
    {
        var a = GuidExtensions.NewComb();
        var b = GuidExtensions.NewComb();
        a.Should().NotBe(b);
    }
}
