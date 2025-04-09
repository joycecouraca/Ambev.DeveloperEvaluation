using Ambev.DeveloperEvaluation.Application.Sales.Commands.Update;
using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.Update;

public class UpdateSaleCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IContextUserProvider> _contextUserProviderMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Fixture _fixture = new();

    private readonly Mock<ISaleRepository> _salesRepoMock = new();
    private readonly UpdateSaleCommandHandler _handler;

    public UpdateSaleCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.Sales).Returns(_salesRepoMock.Object);
        _handler = new UpdateSaleCommandHandler(_unitOfWorkMock.Object, _contextUserProviderMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateSale_WhenCommandIsValid()
    {
        var customer = _fixture.Create<User>();
        var creator = _fixture.Create<User>();
        var productId = Guid.NewGuid();

        var sale = Sale.Create(customer, creator, DateTime.UtcNow, "Loja 1");
        var product = new Product("Coca", 5, "Bebida", 50, ProductCategory.Others);
        sale.AddItems([new SaleItem(product, 2)]); 

        var command = new UpdateSaleCommand
        {
            SaleId = sale.Id,
            CustomerId = customer.Id,
            SoldAt = DateTime.UtcNow,
            BranchName = "Loja Atualizada",
            Items = new List<SaleItemDto>
        {
            new() { ProductId = product.Id, Quantity = 3 }
        }
        };

        _salesRepoMock.Setup(r => r.GetByIdAsync(command.SaleId, It.IsAny<CancellationToken>())).ReturnsAsync(sale);
        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        _mapperMock.Setup(m => m.Map<SaleDto>(It.IsAny<Sale>())).Returns(_fixture.Create<SaleDto>());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSaleNotFound()
    {
        var command = _fixture.Create<UpdateSaleCommand>();
        _salesRepoMock.Setup(s => s.GetByIdAsync(command.SaleId, It.IsAny<CancellationToken>())).ReturnsAsync((Sale)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Sale not found.");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSaleCannotBeEdited()
    {
        var sale = new Mock<Sale>();
        var user = _fixture.Create<User>();
        sale.Object.Delete(user);

        var command = _fixture.Create<UpdateSaleCommand>();
        _salesRepoMock.Setup(s => s.GetByIdAsync(command.SaleId, It.IsAny<CancellationToken>())).ReturnsAsync(sale.Object);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Sale cannot be edited in its current state.");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSaleCanBeCancelled()
    {
        var sale = new Mock<Sale>();

        var user = _fixture.Create<User>();
        sale.Object.Cancel(user);

        var command = _fixture.Create<UpdateSaleCommand>();
        _salesRepoMock.Setup(s => s.GetByIdAsync(command.SaleId, It.IsAny<CancellationToken>())).ReturnsAsync(sale.Object);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Sale be Canceled in its current state.");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCustomerNotFound()
    {
        var sale = new Mock<Sale>();        

        var command = _fixture.Create<UpdateSaleCommand>();

        _salesRepoMock.Setup(s => s.GetByIdAsync(command.SaleId, It.IsAny<CancellationToken>())).ReturnsAsync(sale.Object);
        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>())).ReturnsAsync((User)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Customer not found.");
    }

    [Fact]
    public async Task Handle_ShouldNotChangeItems_WhenItemsListIsNull()
    {
        var sale = new Mock<Sale>();        
        var customer = _fixture.Create<User>();

        var command = _fixture.Build<UpdateSaleCommand>()
            .With(c => c.CustomerId, customer.Id)
            .With(c => c.Items, (List<SaleItemDto>?)null)
            .Create();

        _salesRepoMock.Setup(s => s.GetByIdAsync(command.SaleId, It.IsAny<CancellationToken>())).ReturnsAsync(sale.Object);
        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        _mapperMock.Setup(m => m.Map<SaleDto>(It.IsAny<Sale>())).Returns(_fixture.Create<SaleDto>());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
