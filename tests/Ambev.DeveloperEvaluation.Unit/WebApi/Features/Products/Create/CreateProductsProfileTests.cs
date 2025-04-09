using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Create;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Create;

public class CreateProductsProfileTests
{
    private readonly IMapper _mapper;

    public CreateProductsProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CreateProductsProfile>());
        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Should_Map_CreateProductsRequest_To_CreateProductsCommand()
    {
        var request = new CreateProductsRequest
        {
            Name = "Laptop",
            Description = "Powerful laptop",
            Price = 2999.99m,
            Quantity = 5,
            Category = "Electronics"
        };

        var command = _mapper.Map<CreateProductsCommand>(request);

        command.Should().NotBeNull();
        command.Name.Should().Be("Laptop");
        command.Description.Should().Be("Powerful laptop");
        command.Price.Should().Be(2999.99m);
        command.Quantity.Should().Be(5);
        command.Category.Should().Be(ProductCategory.Electronics);
    }
}