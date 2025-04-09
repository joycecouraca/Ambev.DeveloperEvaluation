using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.Create;

public class CreateSalesCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ISaleRepository> _salesRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IContextUserProvider> _contextUserProviderMock = new();
    private readonly Mock<IUser> _userMock = new();

    public CreateSalesCommandHandlerTests()
    {
        _userMock.Setup(u => u.Id).Returns(Guid.NewGuid());
        _userMock.Setup(u => u.Username).Returns("test_user");
        _userMock.Setup(u => u.Role).Returns("Admin");

        _contextUserProviderMock.Setup(x => x.GetCurrentUser())
            .Returns(_userMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateSale_WhenCommandIsValid()
    {
        var fixture = new Fixture();

        var customer = fixture.Create<User>();
        var creator = fixture.Create<User>();

        var userMock = new Mock<IUser>();
        userMock.Setup(u => u.Id).Returns(creator.Id);
        userMock.Setup(u => u.Username).Returns("test_user");
        userMock.Setup(u => u.Role).Returns("Admin");

        var contextUserProviderMock = new Mock<IContextUserProvider>();
        contextUserProviderMock.Setup(x => x.GetCurrentUser())
            .Returns(userMock.Object);

        var product = new Product("Guaraná", 5.0m, "Refrigerante", 100, ProductCategory.Others);

        var command = new CreateSalesCommand
        {
            CustomerId = customer.Id,
            SoldAt = DateTime.UtcNow,
            BranchName = "Loja Centro",
            Items = new List<SaleItemUpsertDto>
        {
            new() { ProductId = product.Id, Quantity = 5 }
        }
        };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(creator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(creator);
        unitOfWorkMock.Setup(u => u.Products.FindAllAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { product });

        unitOfWorkMock.Setup(u => u.Sales).Returns(_salesRepoMock.Object);


        var saleDto = fixture.Create<SaleDto>();

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<SaleDto>(It.IsAny<Sale>()))
            .Returns(saleDto);

        var handler = new CreateSalesCommandHandler(unitOfWorkMock.Object, mapperMock.Object, contextUserProviderMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(saleDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCustomerNotFound()
    {
        var command = _fixture.Create<CreateSalesCommand>();

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null!);

        var handler = new CreateSalesCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _contextUserProviderMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Customer not found.");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCreatorUserNotFound()
    {
        var customer = _fixture.Create<User>();
        var command = _fixture.Create<CreateSalesCommand>();

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(_userMock.Object.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null!);

        var handler = new CreateSalesCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _contextUserProviderMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User creating the sale was not found.");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNotFound()
    {
        var customer = _fixture.Create<User>();
        var creator = _fixture.Create<User>();
        var missingProductId = Guid.NewGuid();

        var command = new CreateSalesCommand
        {
            CustomerId = customer.Id,
            SoldAt = DateTime.UtcNow,
            BranchName = "Loja A",
            Items = new List<SaleItemUpsertDto>
            {
                new() { ProductId = missingProductId, Quantity = 3 }
            }
        };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(_userMock.Object.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(creator);

        _unitOfWorkMock.Setup(u => u.Products.FindAllAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>()); 

        var handler = new CreateSalesCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _contextUserProviderMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Product with ID");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenQuantityExceedsLimit()
    {
        var customer = _fixture.Create<User>();
        var creator = _fixture.Create<User>();

        var product = new Product("Guaraná", 5.0m, "Refrigerante", 100, ProductCategory.Others);

        var command = new CreateSalesCommand
        {
            CustomerId = customer.Id,
            SoldAt = DateTime.UtcNow,
            BranchName = "Loja B",
            Items = new List<SaleItemUpsertDto>
            {
                new() { ProductId = product.Id, Quantity = 25 }
            }
        };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(_userMock.Object.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(creator);

        _unitOfWorkMock.Setup(u => u.Products.FindAllAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { product });

        var handler = new CreateSalesCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _contextUserProviderMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Cannot sell more than 20");
    }
}