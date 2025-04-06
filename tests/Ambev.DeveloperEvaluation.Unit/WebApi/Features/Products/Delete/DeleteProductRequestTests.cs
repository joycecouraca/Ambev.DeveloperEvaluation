using Ambev.DeveloperEvaluation.WebApi.Features.Products.Delete;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Delete;

public class DeleteProductRequestTests
{
    [Fact]
    public void DeleteProductRequest_Should_Set_Id_Correctly()
    {
        var productId = Guid.NewGuid();

        var request = new DeleteProductRequest { Id = productId };

        request.Should().NotBeNull();
        request.Id.Should().Be(productId);
    }
}