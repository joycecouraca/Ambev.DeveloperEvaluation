using Ambev.DeveloperEvaluation.WebApi.Features.Products.Create;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Create;

public class CreateProductsRequestTests
{
    [Fact]
    public void CreateProductsRequest_Should_Set_All_Properties_Correctly()
    {
        const string name = "Product 1";
        const decimal price = 49.99m;
        const string description = "A test product";
        const int quantity = 10;
        const string category = "Electronics";

        var request = new CreateProductsRequest
        {
            Name = name,
            Price = price,
            Description = description,
            Quantity = quantity,
            Category = category
        };

        request.Name.Should().Be(name);
        request.Price.Should().Be(price);
        request.Description.Should().Be(description);
        request.Quantity.Should().Be(quantity);
        request.Category.Should().Be(category);
    }
}
