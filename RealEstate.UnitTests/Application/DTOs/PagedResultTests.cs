using FluentAssertions;
using NUnit.Framework;
using RealEstate.Application.DTOs.Common;

namespace RealEstate.UnitTests.Application.DTOs;

public class PagedResultTests
{
    [Test]
    public void TotalPages_WithExactDivision_ShouldReturnCorrectValue()
    {
        var pagedResult = new PagedResult<string>
        {
            TotalCount = 100,
            PageSize = 10
        };

        pagedResult.TotalPages.Should().Be(10);
    }

    [Test]
    public void TotalPages_WithRemainder_ShouldRoundUp()
    {
        var pagedResult = new PagedResult<string>
        {
            TotalCount = 101,
            PageSize = 10
        };

        pagedResult.TotalPages.Should().Be(11);
    }

    [Test]
    public void TotalPages_WithZeroCount_ShouldReturnZero()
    {
        var pagedResult = new PagedResult<string>
        {
            TotalCount = 0,
            PageSize = 10
        };

        pagedResult.TotalPages.Should().Be(0);
    }

    [Test]
    public void HasNextPage_WhenOnLastPage_ShouldReturnFalse()
    {
        var pagedResult = new PagedResult<string>
        {
            Page = 5,
            TotalCount = 50,
            PageSize = 10
        };

        pagedResult.HasNextPage.Should().BeFalse();
    }

    [Test]
    public void HasNextPage_WhenNotOnLastPage_ShouldReturnTrue()
    {
        var pagedResult = new PagedResult<string>
        {
            Page = 3,
            TotalCount = 50,
            PageSize = 10
        };

        pagedResult.HasNextPage.Should().BeTrue();
    }

    [Test]
    public void HasPreviousPage_WhenOnFirstPage_ShouldReturnFalse()
    {
        var pagedResult = new PagedResult<string>
        {
            Page = 1,
            TotalCount = 50,
            PageSize = 10
        };

        pagedResult.HasPreviousPage.Should().BeFalse();
    }

    [Test]
    public void HasPreviousPage_WhenNotOnFirstPage_ShouldReturnTrue()
    {
        var pagedResult = new PagedResult<string>
        {
            Page = 3,
            TotalCount = 50,
            PageSize = 10
        };

        pagedResult.HasPreviousPage.Should().BeTrue();
    }

    [Test]
    public void Items_ShouldBeInitializedAsEmptyList()
    {
        var pagedResult = new PagedResult<int>();

        pagedResult.Items.Should().NotBeNull();
        pagedResult.Items.Should().BeEmpty();
    }

    [Test]
    public void Properties_ShouldAllowSettingAndGetting()
    {
        var items = new List<string> { "item1", "item2", "item3" };
        var pagedResult = new PagedResult<string>
        {
            Items = items,
            TotalCount = 23,
            Page = 2,
            PageSize = 5
        };

        pagedResult.Items.Should().BeEquivalentTo(items);
        pagedResult.TotalCount.Should().Be(23);
        pagedResult.Page.Should().Be(2);
        pagedResult.PageSize.Should().Be(5);
        pagedResult.TotalPages.Should().Be(5); // Ceiling of 23/5
        pagedResult.HasNextPage.Should().BeTrue(); // Page 2 of 5
        pagedResult.HasPreviousPage.Should().BeTrue(); // Not on first page
    }

    [Test]
    public void EdgeCase_SingleItemSinglePage_ShouldWorkCorrectly()
    {
        var pagedResult = new PagedResult<string>
        {
            Items = new List<string> { "single item" },
            TotalCount = 1,
            Page = 1,
            PageSize = 10
        };

        pagedResult.TotalPages.Should().Be(1);
        pagedResult.HasNextPage.Should().BeFalse();
        pagedResult.HasPreviousPage.Should().BeFalse();
    }
}