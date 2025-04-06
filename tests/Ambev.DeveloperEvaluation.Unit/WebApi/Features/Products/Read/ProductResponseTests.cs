using Ambev.DeveloperEvaluation.WebApi.Features.Products.Read;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Read;

public class ProductResponseTests
{
    [Fact]
    public void ProductResponse_Should_Set_And_Get_Properties_Correctly()
    {
        const string name = "Sample Product";
        const string category = "Beverages";
        const decimal price = 29.99m;
        const string description = "A refreshing beverage";
        const int quantity = 10;

        var id = Guid.NewGuid();

        var response = new ProductResponse
        {
            Id = id,
            Name = name,
            Category = category,
            Price = price,
            Description = description,
            Quantity = quantity
        };

        response.Id.Should().Be(id);
        response.Name.Should().Be(name);
        response.Category.Should().Be(category);
        response.Price.Should().Be(price);
        response.Description.Should().Be(description);
        response.Quantity.Should().Be(quantity);
    }
}
