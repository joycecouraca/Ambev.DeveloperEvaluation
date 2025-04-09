using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.SaleItem;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.Cancel.SaleItems;

public class CancelSaleItemCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IContextUserProvider> _userProviderMock;
    private readonly CancelSaleItemCommandHandler _handler;

    public CancelSaleItemCommandHandlerTests()
    {
        _fixture = new Fixture();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _userProviderMock = _fixture.Freeze<Mock<IContextUserProvider>>();
        _handler = new CancelSaleItemCommandHandler(_unitOfWorkMock.Object, _userProviderMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCancelSelectedItems_WhenValid()
    {
        var user = _fixture.Create<User>();
        var product = new Product("Guaraná", 4.5m, "Bebida", 50, ProductCategory.Others);
        var item1 = new SaleItem(product, 2);
        var item2 = new SaleItem(product, 3);
        var sale = Sale.Create(user, user, DateTime.UtcNow, "Filial XPTO");

        sale.AddItems(item1, item2);

        _userProviderMock.Setup(x => x.GetCurrentUser()).Returns(user);
        _unitOfWorkMock.Setup(x => x.Sales.GetByIdWithItemsAsync(sale.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(sale);

        _unitOfWorkMock.Setup(x => x.Users.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);

        var command = new CancelSaleItemsCommand(sale.Id, new List<Guid> { item2.Id });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.SaleId.Should().Be(sale.Id);        
        result.Value.CancelledItems[1].ItemId.Should().Be(item2.Id);
        item2.Status.Should().Be(SaleItemStatus.Cancelled);
        item1.Status.Should().Be(SaleItemStatus.Cancelled);

        _unitOfWorkMock.Verify(u => u.Sales.Update(sale), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNull()
    {
        _userProviderMock.Setup(x => x.GetCurrentUser()).Returns((User?)null!);
        var command = _fixture.Create<CancelSaleItemsCommand>();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("CurrentUser not found.");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSaleNotFound()
    {
        var user = _fixture.Create<User>();
        _userProviderMock.Setup(x => x.GetCurrentUser()).Returns(user);
        _unitOfWorkMock.Setup(x => x.Sales.GetByIdWithItemsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Sale?)null);

        _unitOfWorkMock.Setup(x => x.Users.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);

        var command = _fixture.Create<CancelSaleItemsCommand>();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Sale not found.");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenItemIdsDoNotMatch()
    {
        var user = _fixture.Create<User>();
        var product = new Product("Suco", 6.0m, "Bebidas", 40, ProductCategory.Others);
        var item = new SaleItem(product, 1);
        var sale = Sale.Create(user, user, DateTime.UtcNow, "Loja Teste");
        sale.AddItems(item);

        _userProviderMock.Setup(x => x.GetCurrentUser()).Returns(user);
        _unitOfWorkMock.Setup(x => x.Sales.GetByIdWithItemsAsync(sale.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(sale);
        _unitOfWorkMock.Setup(x => x.Users.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);


        var command = new CancelSaleItemsCommand(sale.Id, new List<Guid> { Guid.NewGuid() });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No valid items found for cancellation.");
    }
}
