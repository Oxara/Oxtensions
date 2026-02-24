// -----------------------------------------------------------------------
// <copyright file="DataTableExtensionsTests.cs" company="Oxtensions">
//   MIT License - https://github.com/oxara/oxtensions
// </copyright>
// -----------------------------------------------------------------------

#nullable enable

using System.Data;
using FluentAssertions;
using Oxtensions.DataTable;
using Xunit;

namespace Oxtensions.Tests.DataTable;

public sealed record PersonRecord
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Salary { get; set; }
}

public sealed class DataTableExtensionsTests
{
    private static System.Data.DataTable BuildPersonTable()
    {
        var table = new System.Data.DataTable("Persons");
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Salary", typeof(decimal));

        table.Rows.Add(1, "Alice", 5000m);
        table.Rows.Add(2, "Bob", 7500m);
        table.Rows.Add(3, "Charlie", 9000m);

        return table;
    }

    // ── HasRows ───────────────────────────────────────────────────────────────

    [Fact]
    public void HasRows_TableWithRows_ReturnsTrue()
        => BuildPersonTable().HasRows().Should().BeTrue();

    [Fact]
    public void HasRows_EmptyTable_ReturnsFalse()
        => new System.Data.DataTable().HasRows().Should().BeFalse();

    [Fact]
    public void HasRows_NullTable_ReturnsFalse()
        => ((System.Data.DataTable?)null).HasRows().Should().BeFalse();

    // ── ToList<T> ─────────────────────────────────────────────────────────────

    [Fact]
    public void ToList_ValidTable_ReturnsMappedObjects()
    {
        var result = BuildPersonTable().ToList<PersonRecord>();
        result.Should().HaveCount(3);
        result[0].Id.Should().Be(1);
        result[0].Name.Should().Be("Alice");
        result[1].Salary.Should().Be(7500m);
    }

    [Fact]
    public void ToList_NullTable_ThrowsArgumentNullException()
    {
        var act = () => ((System.Data.DataTable)null!).ToList<PersonRecord>();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToList_EmptyTable_ReturnsEmptyList()
    {
        var empty = new System.Data.DataTable();
        empty.Columns.Add("Id", typeof(int));
        empty.ToList<PersonRecord>().Should().BeEmpty();
    }

    // ── ToJson ────────────────────────────────────────────────────────────────

    [Fact]
    public void ToJson_ValidTable_ReturnsJsonArray()
    {
        var json = BuildPersonTable().ToJson();
        json.Should().StartWith("[");
        json.Should().Contain("Alice");
        json.Should().Contain("7500");
    }

    [Fact]
    public void ToJson_EmptyTable_ReturnsEmptyArray()
    {
        var empty = new System.Data.DataTable();
        empty.Columns.Add("Id", typeof(int));
        empty.ToJson().Should().Be("[]");
    }

    // ── ToDictionary ──────────────────────────────────────────────────────────

    [Fact]
    public void ToDictionary_ValidTable_ReturnsMappedDictionary()
    {
        var dict = BuildPersonTable().ToDictionary<int, string>("Id", "Name");
        dict.Should().HaveCount(3);
        dict[1].Should().Be("Alice");
        dict[2].Should().Be("Bob");
    }

    [Fact]
    public void ToDictionary_NullTable_ThrowsArgumentNullException()
    {
        var act = () => ((System.Data.DataTable)null!).ToDictionary<int, string>("Id", "Name");
        act.Should().Throw<ArgumentNullException>();
    }

    // ── ForEach ───────────────────────────────────────────────────────────────

    [Fact]
    public void ForEach_ValidTable_ExecutesForEachRow()
    {
        int count = 0;
        BuildPersonTable().ForEach(_ => count++);
        count.Should().Be(3);
    }

    [Fact]
    public void ForEach_NullTable_ThrowsArgumentNullException()
    {
        var act = () => ((System.Data.DataTable)null!).ForEach(_ => { });
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ForEach_AccumulatesRowData_Correctly()
    {
        var ids = new List<int>();
        BuildPersonTable().ForEach(row => ids.Add((int)row["Id"]));
        ids.Should().Equal(1, 2, 3);
    }
}
