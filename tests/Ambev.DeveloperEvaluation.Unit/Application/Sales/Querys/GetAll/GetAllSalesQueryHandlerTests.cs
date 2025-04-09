using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.Querys.GetAll;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Querys.GetAll;

public class GetAllSalesQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<GetAllSalesQueryHandler>> _loggerMock = new();
    private readonly Fixture _fixture = new();
    private readonly GetAllSalesQueryHandler _handler;

    public GetAllSalesQueryHandlerTests()
    {
        _handler = new GetAllSalesQueryHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedSales_WhenQueryIsValid()
    {
        var query = new GetAllSalesQuery(1, 10, default);
        var sales = _fixture.Create<PaginatedList<Sale>>();
        var salesDto = _fixture.Create<PaginatedList<SaleDto>>();

        _unitOfWorkMock.Setup(u => u.Sales.GetPaginatedAsync(
            query.Page,
            query.PageSize,
            null,
            It.IsAny<Func<IQueryable<Sale>, IOrderedQueryable<Sale>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(sales);

        _mapperMock.Setup(m => m.Map<PaginatedList<SaleDto>>(sales))
            .Returns(salesDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(salesDto);
    }

    [Fact]
    public async Task Handle_ShouldApplySearchFilter_WhenSearchQueryProvided()
    {
        var query = new GetAllSalesQuery(1, 10, "john");

        var sales = _fixture.Create<PaginatedList<Sale>>();
        var salesDto = _fixture.Create<PaginatedList<SaleDto>>();

        _unitOfWorkMock.Setup(u => u.Sales.GetPaginatedAsync(
            query.Page,
            query.PageSize,
            It.IsAny<Expression<Func<Sale, bool>>>(),
            It.IsAny<Func<IQueryable<Sale>, IOrderedQueryable<Sale>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(sales);

        _mapperMock.Setup(m => m.Map<PaginatedList<SaleDto>>(sales))
            .Returns(salesDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(salesDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoSalesFound()
    {
        var query = new GetAllSalesQuery(1, 10, default);

        var emptySales = new PaginatedList<Sale>
        {
            Items = [],
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = 5
        };

        var emptySalesDto = new PaginatedList<SaleDto>
        {
            Items = [],
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = 5
        };

        _unitOfWorkMock.Setup(u => u.Sales.GetPaginatedAsync(
            query.Page,
            query.PageSize,
            null,
            It.IsAny<Func<IQueryable<Sale>, IOrderedQueryable<Sale>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptySales);

        _mapperMock.Setup(m => m.Map<PaginatedList<SaleDto>>(emptySales))
            .Returns(emptySalesDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldPassPaginationParametersCorrectly()
    {
        var query = new GetAllSalesQuery(2, 5, default);
        var sales = _fixture.Create<PaginatedList<Sale>>();
        var salesDto = _fixture.Create<PaginatedList<SaleDto>>();

        _unitOfWorkMock.Setup(u => u.Sales.GetPaginatedAsync(
            query.Page,
            query.PageSize,
            It.IsAny<Expression<Func<Sale, bool>>>(),
            It.IsAny<Func<IQueryable<Sale>, IOrderedQueryable<Sale>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(sales)
            .Verifiable();

        _mapperMock.Setup(m => m.Map<PaginatedList<SaleDto>>(sales))
            .Returns(salesDto);

        await _handler.Handle(query, CancellationToken.None);

        _unitOfWorkMock.Verify();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionThrown()
    {
        var query = new GetAllSalesQuery(1, 10, default);
        _unitOfWorkMock.Setup(u => u.Sales.GetPaginatedAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Expression<Func<Sale, bool>>>(),
            It.IsAny<Func<IQueryable<Sale>, IOrderedQueryable<Sale>>>(),
            It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Something went wrong"));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Something went wrong");
    }
}
