using Ambev.DeveloperEvaluation.WebApi.Features.Products.Read.GetById;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Read.GetById;

public class GetProductByIdRequestTests
{
    [Fact]
    public void Should_Set_And_Get_Id_Correctly()
    {
        var expectedId = Guid.NewGuid();

        var request = new GetProductByIdRequest
        {
            Id = expectedId
        };

        request.Id.Should().Be(expectedId);
    }
}