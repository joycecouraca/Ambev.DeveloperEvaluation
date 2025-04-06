using Ambev.DeveloperEvaluation.WebApi.Features.Products.Update;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Update;

public class UpdateProductsResponseTests
{
    [Fact]
    public void Should_Create_Response_With_Valid_Properties()
    {
        var response = new UpdateProductsResponse
        {
            Id = Guid.NewGuid(),
            Name = "Updated Product",
            Category = "Electronics",
            Price = 99.99m,
            Description = "Updated description",
            Quantity = 10
        };

        response.Name.Should().Be("Updated Product");
        response.Category.Should().Be("Electronics");
        response.Price.Should().Be(99.99m);
        response.Description.Should().Be("Updated description");
        response.Quantity.Should().Be(10);
        response.Id.Should().NotBe(Guid.Empty);
    }
}
