using Ambev.DeveloperEvaluation.Application.Products.Commands.Update;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Update.Dtos;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Read;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Update;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Update;

public class UpdateProductsProfileTests
{
    private readonly IMapper _mapper;

    public UpdateProductsProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UpdateProductsProfile>());

        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Should_Map_UpdateProductsRequest_To_UpdateProductsCommand()
    {
        var request = new UpdateProductsRequest
        {
            Id = Guid.NewGuid(),
            Name = "Updated Product",
            Price = 49.99m,
            Description = "Updated description",
            Quantity = 5,
            Category = "Others"
        };

        var command = _mapper.Map<UpdateProductsCommand>(request);

        command.Id.Should().Be(request.Id);
        command.Name.Should().Be(request.Name);
        command.Price.Should().Be(request.Price);
        command.Description.Should().Be(request.Description);
        command.Quantity.Should().Be(request.Quantity);
        command.Category.ToString().Should().Be(request.Category);
    }

    [Fact]
    public void Should_Map_UpdateProductDto_To_UpdateProductsResponse()
    {
        var dto = new UpdateProductDto
        {
            Id = Guid.NewGuid(),
            Name = "Product X",
            Description = "A product",
            Price = 12.5m,
            Quantity = 7,
            Category = nameof(ProductCategory.Groceries)
        };

        var response = _mapper.Map<UpdateProductsResponse>(dto);

        response.Id.Should().Be(dto.Id);
        response.Name.Should().Be(dto.Name);
        response.Description.Should().Be(dto.Description);
        response.Price.Should().Be(dto.Price);
        response.Quantity.Should().Be(dto.Quantity);
        response.Category.Should().Be(dto.Category);
    }

    [Fact]
    public void Should_Map_GetProductDto_To_And_From_ProductResponse()
    {
        var dto = new GetProductDto
        {
            Id = Guid.NewGuid(),
            Name = "Another Product",
            Description = "Desc",
            Price = 99.9m,
            Quantity = 15,
            Category = nameof(ProductCategory.Groceries)
        };

        var response = _mapper.Map<ProductResponse>(dto);
        var reversed = _mapper.Map<GetProductDto>(response);

        response.Id.Should().Be(dto.Id);
        response.Name.Should().Be(dto.Name);
        response.Description.Should().Be(dto.Description);
        response.Price.Should().Be(dto.Price);
        response.Quantity.Should().Be(dto.Quantity);
        response.Category.Should().Be(dto.Category);

        reversed.Should().BeEquivalentTo(dto);
    }
}
