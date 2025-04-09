using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Update;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.Commands.Update;

public class UpdateProductsCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<CreateProductsCommandHandler>> _loggerMock = new();

    private readonly UpdateProductsCommandHandler _handler;

    public UpdateProductsCommandHandlerTests()
    {
        _handler = new UpdateProductsCommandHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNotFound()
    {
        var command = CreateCommand();
        _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_ShouldUpdateProduct_WhenValidCommand()
    {
        var command = CreateCommand();
        var product = new Product(command.Name, command.Price, command.Description, command.Quantity, command.Category);
        var productDto = new ProductDto();

        _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _mapperMock.Setup(m => m.Map(command, product));
        _unitOfWorkMock.Setup(u => u.CommitChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(productDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionThrown()
    {
        var command = CreateCommand();

        _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Unexpected error");
    }

    private static UpdateProductsCommand CreateCommand()
    {
        const string name = "Updated Product";
        const decimal price = 99.99m;
        const string description = "Updated description";
        const int quantity = 15;
        const ProductCategory category = ProductCategory.Others;

        return new UpdateProductsCommand
        {
            Id = Guid.NewGuid(),
            Name = name,
            Price = price,
            Description = description,
            Quantity = quantity,
            Category = category
        };
    }
}
