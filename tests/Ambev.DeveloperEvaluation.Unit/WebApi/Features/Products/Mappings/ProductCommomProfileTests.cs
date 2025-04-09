using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Common.Mappings;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Common.Response;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Mappings;

public class ProductCommomProfileTests
{
    private readonly IMapper _mapper;

    public ProductCommomProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductCommomProfile>());
        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Should_Map_ProductDto_To_ProductsResponse()
    {
        var dto = new ProductDto
        {
            Id = Guid.NewGuid(),
            Name = "Smartphone",
            Description = "Latest model",
            Price = 1999.50m,
            Quantity = 8,
            Category = nameof(ProductCategory.Electronics)
        };

        var response = _mapper.Map<ProductResponse>(dto);

        response.Should().NotBeNull();
        response.Id.Should().Be(dto.Id);
        response.Name.Should().Be("Smartphone");
        response.Description.Should().Be("Latest model");
        response.Price.Should().Be(1999.50m);
        response.Quantity.Should().Be(8);
        response.Category.Should().Be("Electronics");
    }
}
