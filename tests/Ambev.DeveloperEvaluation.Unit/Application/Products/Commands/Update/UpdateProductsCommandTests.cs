using Ambev.DeveloperEvaluation.Application.Products.Commands.Update;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.Commands.Update;

public class UpdateProductsCommandTests
{
    [Fact]
    public void Properties_ShouldBeSetCorrectly()
    {
        const string name = "Product X";
        const decimal price = 49.99m;
        const string description = "Description for Product X";
        const int quantity = 20;
        const ProductCategory category = ProductCategory.Others;
        var id = Guid.NewGuid();

        var command = new UpdateProductsCommand
        {
            Id = id,
            Name = name,
            Price = price,
            Description = description,
            Quantity = quantity,
            Category = category
        };

        command.Id.Should().Be(id);
        command.Name.Should().Be(name);
        command.Price.Should().Be(price);
        command.Description.Should().Be(description);
        command.Quantity.Should().Be(quantity);
        command.Category.Should().Be(category);
    }
}
