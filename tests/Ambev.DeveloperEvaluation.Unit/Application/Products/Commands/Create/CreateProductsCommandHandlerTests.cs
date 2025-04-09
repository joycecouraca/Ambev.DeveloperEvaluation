using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.Commands.Create;

public class CreateProductsCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<CreateProductsCommandHandler>> _loggerMock = new();
    private readonly CreateProductsCommandHandler _handler;

    public CreateProductsCommandHandlerTests()
    {
        _handler = new CreateProductsCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductAlreadyExists()
    {
        var command = new CreateProductsCommand
        {
            Name = "Beer",
            Price = 10,
            Description = "Cold and fresh",
            Quantity = 5,
            Category = ProductCategory.Others
        };
        _unitOfWorkMock.Setup(u => u.Products.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("A product with the name 'Beer' already exists.");
        _unitOfWorkMock.Verify(u => u.CommitChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        // Verifica o log
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) =>
                    v.ToString()!.Contains("A product with the name") &&
                    v.ToString()!.Contains("Beer")
                ),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProductIsCreated()
    {
        var command = new CreateProductsCommand
        {
            Name = "Beer",
            Price = 10,
            Description = "Cold and fresh",
            Quantity = 5,
            Category = ProductCategory.Automotive
        };
        var product = new Product("Beer", 10, "Cold and fresh", 5, ProductCategory.Automotive);
        var productDto = new ProductDto
        {
            Id = Guid.NewGuid(),
            Name = "Smartphone",
            Description = "Latest model",
            Price = 1999.50m,
            Quantity = 8,
            Category = nameof(ProductCategory.Electronics)
        };

        _unitOfWorkMock.Setup(u => u.Products.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false);

        _mapperMock.Setup(m => m.Map<Product>(command)).Returns(product);
        _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(productDto);
        _unitOfWorkMock.Verify(u => u.Products.Add(product), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionIsThrown()
    {
        var command = new CreateProductsCommand
        {
            Name = "Beer",
            Price = 10,
            Description = "Cold and fresh",
            Quantity = 5,
            Category = ProductCategory.Others
        };

        _unitOfWorkMock.Setup(u => u.Products.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>(), CancellationToken.None))
                       .ReturnsAsync(false);

        _mapperMock.Setup(m => m.Map<Product>(command)).Throws(new Exception("Mapper failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("An unexpected error occurred while creating the product.");
        _unitOfWorkMock.Verify(u => u.CommitChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _loggerMock.Verify(
           x => x.Log(
               LogLevel.Error,
               It.IsAny<EventId>(),
               It.Is<It.IsAnyType>((v, _) =>
                   v.ToString()!.Contains("An error occurred while creating the product")
               ),
               It.IsAny<Exception?>(),
               It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
           Times.Once);
    }
}
