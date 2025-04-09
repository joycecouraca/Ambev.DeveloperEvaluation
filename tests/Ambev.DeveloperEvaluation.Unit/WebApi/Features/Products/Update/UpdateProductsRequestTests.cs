
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Update;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Update;

public class UpdateProductsRequestTests
{
    [Fact]
    public void Should_Create_UpdateProductsRequest_With_All_Properties_Set()
    {
        var id = Guid.NewGuid();
        var request = new UpdateProductsRequest
        {
            Id = id,
            Name = "Updated Product",
            Price = 99.99m,
            Description = "Updated description",
            Quantity = 10,
            Category = "Food"
        };

        request.Id.Should().Be(id);
        request.Name.Should().Be("Updated Product");
        request.Price.Should().Be(99.99m);
        request.Description.Should().Be("Updated description");
        request.Quantity.Should().Be(10);
        request.Category.Should().Be("Food");
    }
}
