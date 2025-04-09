using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.Sale;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.Cancel.Sales;

public class CancelSaleCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IContextUserProvider> _userProviderMock;
    private readonly CancelSaleCommandHandler _handler;


    public CancelSaleCommandHandlerTests()
    {
        _fixture = new Fixture();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _userProviderMock = _fixture.Freeze<Mock<IContextUserProvider>>();
        _handler = new CancelSaleCommandHandler(_unitOfWorkMock.Object, _userProviderMock.Object);

        var user = _fixture.Create<User>();
        _unitOfWorkMock.Setup(x => x.Users.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);
    }

    [Fact]
    public async Task Handle_ShouldCancelSale_WhenValid()
    {
        var user = _fixture.Create<User>();
        var sale = Sale.Create(user, user, DateTime.UtcNow, "Loja A");

        _unitOfWorkMock.Setup(u => u.Sales.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(sale);

        _unitOfWorkMock.Setup(x => x.Users.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);

        _userProviderMock.Setup(c => c.GetCurrentUser()).Returns(user);

        var command = new CancelSaleCommand(sale.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(sale.Id);
        sale.Status.Should().Be(SaleStatus.Cancelled);

        _unitOfWorkMock.Verify(u => u.Sales.Update(sale), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSaleNotFound()
    {
        var id = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.Sales.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((Sale?)null!);

        var command = new CancelSaleCommand(id);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Sale not found");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSaleCannotBeCancelled()
    {
        var user = _fixture.Create<User>();
        var sale = Sale.Create(user, user, DateTime.UtcNow, "Loja B");
        sale.Cancel(user);

        _unitOfWorkMock.Setup(u => u.Sales.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(sale);

        _userProviderMock.Setup(c => c.GetCurrentUser()).Returns(user);

        var command = new CancelSaleCommand(sale.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("This sale cannot be cancelled");
    }
}