using Ambev.DeveloperEvaluation.WebApi.Features.Products.Common.Response;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Create;

public class CreateProductsResponseTests
{
    [Fact]
    public void CreateProductsResponse_Should_Set_All_Properties_Correctly()
    {
        // Arrange
        var response = new ProductResponse
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Price = 99.99m,
            Description = "Test product description",
            Quantity = 10,
            Category = "Electronics"
        };

        // Assert
        response.Id.Should().NotBeEmpty();
        response.Name.Should().Be("Test Product");
        response.Price.Should().Be(99.99m);
        response.Description.Should().Be("Test product description");
        response.Quantity.Should().Be(10);
        response.Category.Should().Be("Electronics");
    }
}
