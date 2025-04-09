using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Update;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Application.Products.Mappings;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Mappings;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.Common;

public class ProductsProfileTests
{
    private readonly IMapper _mapper;

    public ProductsProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductsProfile>();            
        });

        _mapper = config.CreateMapper();
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Should_Map_CreateProductsCommand_To_Product()
    {
        const string name = "Test";
        const string description = "Description";
        const decimal price = 20m;
        const int quantity = 5;

        var command = new CreateProductsCommand
        {
            Name = name,
            Description = description,
            Price = price,
            Quantity = quantity,
            Category = ProductCategory.Others
        };

        var product = _mapper.Map<Product>(command);

        product.Should().NotBeNull();
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Price.Should().Be(price);
        product.Quantity.Should().Be(quantity);
        product.Category.Should().Be(ProductCategory.Others);
    }

    [Fact]
    public void Should_Map_Product_To_ProductDto()
    {
        const string name = "Mapped Product";
        const decimal price = 99.99m;
        const string description = "Mapped description";
        const int quantity = 2;

        var product = new Product(name, price, description, quantity, ProductCategory.Others);

        var dto = _mapper.Map<ProductDto>(product);

        dto.Should().NotBeNull();
        dto.Id.Should().Be(product.Id);
        dto.Name.Should().Be(name);
        dto.Price.Should().Be(price);
        dto.Description.Should().Be(description);
        dto.Quantity.Should().Be(quantity);
        dto.Category.Should().Be(ProductCategory.Others.ToString());
    }

    [Fact]
    public void Should_Map_UpdateProductsCommand_To_Product()
    {
        const string name = "Updated Name";
        const string description = "Updated Description";
        const decimal price = 15.5m;
        const int quantity = 10;

        var command = new UpdateProductsCommand
        {
            Id = Guid.NewGuid(),
            Name = name,
            Price = price,
            Description = description,
            Quantity = quantity,
            Category = ProductCategory.Others
        };

        var product = new Product("Old", 1, "Old", 1, ProductCategory.Others);
        _mapper.Map(command, product);

        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Price.Should().Be(price);
        product.Quantity.Should().Be(quantity);
        product.Category.Should().Be(ProductCategory.Others);
    }

    [Fact]
    public void Should_Map_PaginatedList_Of_Product_To_ProductDto()
    {
        var products = new List<Product>
        {
            new("Product 1", 10, "Desc 1", 1, ProductCategory.Others),
            new("Product 2", 20, "Desc 2", 2, ProductCategory.Others)
        };

        var paginated = new PaginatedList<Product>
        {
            Items = products,
            Page = 1,
            PageSize = 2,
            TotalCount = 2
        };

        var dtoPaginated = _mapper.Map<PaginatedList<ProductDto>>(paginated);

        dtoPaginated.Should().NotBeNull();
        dtoPaginated.Items.Should().HaveCount(2);
        dtoPaginated.TotalCount.Should().Be(2);
        dtoPaginated.Page.Should().Be(1);
        dtoPaginated.PageSize.Should().Be(2);
        dtoPaginated.Items[0].Name.Should().Be("Product 1");
        dtoPaginated.Items[1].Name.Should().Be("Product 2");
    }
}
