using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.Commands.Create;

public class CreateProductsCommandTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        const string name = "Coca-Cola";
        const decimal price = 8.99m;
        const string description = "Refrigerante gelado";
        const int quantity = 20;
        const ProductCategory category = ProductCategory.Others;

        var command = new CreateProductsCommand
        {
            Name = name,
            Price = price,
            Description = description,
            Quantity = quantity,
            Category = category
        };

        command.Name.Should().Be(name);
        command.Price.Should().Be(price);
        command.Description.Should().Be(description);
        command.Quantity.Should().Be(quantity);
        command.Category.Should().Be(category);
    }
}
