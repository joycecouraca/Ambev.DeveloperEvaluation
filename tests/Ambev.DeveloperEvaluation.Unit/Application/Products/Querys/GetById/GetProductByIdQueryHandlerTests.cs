using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Application.Products.Query.GetById;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.Querys.GetById;

public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<CreateProductsCommandHandler>> _loggerMock = new();

    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _handler = new GetProductByIdQueryHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProductExists()
    {
        const string name = "Test Product";
        const decimal price = 9.99m;
        const string description = "Test Description";
        const int quantity = 10;

        var id = Guid.NewGuid();
        var query = new GetProductByIdQuery(id);

        var product = new Product(name, price, description, quantity, ProductCategory.Others);
        var dto = new ProductDto { Id = id, Name = name };

        _unitOfWorkMock.Setup(x => x.Products.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(product);

        _mapperMock.Setup(x => x.Map<ProductDto>(product)).Returns(dto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(dto);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNotFound()
    {
        var id = Guid.NewGuid();
        var query = new GetProductByIdQuery(id);

        _unitOfWorkMock.Setup(x => x.Products.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync((Product?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"Product with the id {id} not found.");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionThrown()
    {
        var id = Guid.NewGuid();
        var query = new GetProductByIdQuery(id);

        _unitOfWorkMock.Setup(x => x.Products.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(new Exception("database failure"));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Failed to load products: database failure");
    }
}
