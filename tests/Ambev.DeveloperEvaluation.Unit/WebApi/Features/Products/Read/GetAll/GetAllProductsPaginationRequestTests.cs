using Ambev.DeveloperEvaluation.WebApi.Features.Products.Read.GetAll;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Read.GetAll;

public class GetAllProductsPaginationRequestTests
{
    [Fact]
    public void Should_Assign_Properties_Correctly()
    {
        var request = new GetAllProductsPaginationRequest
        {
            Page = 2,
            PageSize = 50,
            Search = "Bebidas"
        };

        request.Page.Should().Be(2);
        request.PageSize.Should().Be(50);
        request.Search.Should().Be("Bebidas");
    }
}