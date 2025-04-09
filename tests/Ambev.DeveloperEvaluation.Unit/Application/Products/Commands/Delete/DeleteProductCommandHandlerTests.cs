using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Delete;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.Commands.Delete;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<CreateProductsCommandHandler>> _loggerMock = new();
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        _handler = new DeleteProductCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProductIsFoundAndDisabled()
    {
        var productId = Guid.NewGuid();
        var product = new Product("Test", 10, "Desc", 5, ProductCategory.Automotive);
        typeof(Product).GetProperty(nameof(Product.Id))!.SetValue(product, productId);

        _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(product);

        var result = await _handler.Handle(new DeleteProductsCommand(productId), CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.Products.Update(product), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitChangesAsync(CancellationToken.None), Times.Once);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(productId);
        product.Active.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNotFound()
    {
        var productId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Product?)null!);

        var result = await _handler.Handle(new DeleteProductsCommand(productId), CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.Products.Update(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitChangesAsync(CancellationToken.None), Times.Never);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionIsThrown()
    {
        var productId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new Exception("DB error"));

        var result = await _handler.Handle(new DeleteProductsCommand(productId), CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.Products.Update(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitChangesAsync(CancellationToken.None), Times.Never);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Unexpected error");
    }
}
