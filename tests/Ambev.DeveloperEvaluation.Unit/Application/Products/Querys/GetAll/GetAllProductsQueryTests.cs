using Ambev.DeveloperEvaluation.Application.Products.Query.GetAll;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.Querys.GetAll;

public class GetAllProductsQueryTests
{
    [Fact]
    public void Constructor_ShouldAssignPropertiesCorrectly()
    {
        const int page = 2;
        const int pageSize = 15;
        const string search = "product";

        var query = new GetAllProductsQuery
        {
            Page = page,
            PageSize = pageSize,
            Search = search
        };

        query.Page.Should().Be(page);
        query.PageSize.Should().Be(pageSize);
        query.Search.Should().Be(search);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var query = new GetAllProductsQuery();

        const int page = 3;
        const int pageSize = 25;
        const string search = "any";

        query.Page = page;
        query.PageSize = pageSize;
        query.Search = search;

        query.Page.Should().Be(page);
        query.PageSize.Should().Be(pageSize);
        query.Search.Should().Be(search);
    }
}
