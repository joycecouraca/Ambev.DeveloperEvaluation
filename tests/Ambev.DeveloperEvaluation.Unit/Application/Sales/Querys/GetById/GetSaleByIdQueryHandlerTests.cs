using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.Querys.GetById;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Querys.GetById;

public class GetSaleByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<GetSaleByIdQueryHandler>> _loggerMock = new();
    private readonly IFixture _fixture = new Fixture();
    private readonly GetSaleByIdQueryHandler _handler;

    public GetSaleByIdQueryHandlerTests()
    {
        _handler = new GetSaleByIdQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSale_WhenSaleExists()
    {
        var sale = _fixture.Create<Sale>();
        var dto = _fixture.Create<SaleDto>();
        var query = new GetSaleByIdQuery(sale.Id);

        _unitOfWorkMock
            .Setup(u => u.Sales.GetByIdWithItemsAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        _mapperMock.Setup(m => m.Map<SaleDto>(sale)).Returns(dto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSaleNotFound()
    {
        var query = new GetSaleByIdQuery(Guid.NewGuid());

        _unitOfWorkMock
            .Setup(u => u.Sales.GetByIdWithItemsAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionThrown()
    {
        var query = new GetSaleByIdQuery(Guid.NewGuid());

        _unitOfWorkMock
            .Setup(u => u.Sales.GetByIdWithItemsAsync(query.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Failed to load sale");
    }
}
