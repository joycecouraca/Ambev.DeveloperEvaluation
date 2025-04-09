using Ambev.DeveloperEvaluation.Application.Common.Abstractions;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.Sale;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.SaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.Querys.GetAll;
using Ambev.DeveloperEvaluation.Application.Sales.Querys.GetById;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Cancel.Sale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Cancel.SaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common.Response;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Read.GetAll;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Read.GetById;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales;

public class SalesControllerTests
{
    private readonly Mock<IRequestDispatcher> _dispatcherMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly SalesController _controller;

    public SalesControllerTests()
    {
        _dispatcherMock = new Mock<IRequestDispatcher>();
        _mapperMock = new Mock<IMapper>();
        _controller = new SalesController(_mapperMock.Object, _dispatcherMock.Object);
    }

    [Fact]
    public async Task CreateSale_Should_Return_Created_When_Successful()
    {
        var request = new CreateSalesRequest
        {
            CustomerId = Guid.NewGuid(),
            BranchName = "Branch X",
            SoldAt = DateTime.UtcNow,
            Items =
            [
                new() { ProductId = Guid.NewGuid(), Quantity = 2 }
            ]
        };

        var resultDto = new SaleDto
        {
            Id = Guid.NewGuid(),
            SaleNumber = 123456,
            SoldAt = DateTime.UtcNow,
            TotalSaleAmount = 250.50m,
            BranchName = "Branch X",
            CustomerName = "Cliente Teste",
            Status = "Pending",
            Items = []
        };

        var mappedResponse = new CreateSalesResponse
        {
            Id = resultDto.Id,
            SaleNumber = resultDto.SaleNumber,
            SoldAt = resultDto.SoldAt,
            TotalSaleAmount = resultDto.TotalSaleAmount,
            BranchName = resultDto.BranchName,
            CustomerName = resultDto.CustomerName,
            Status = resultDto.Status,
            Items = []
        };

        _dispatcherMock
            .Setup(x => x.SendValidatedAsync<CreateSalesRequest, CreateSalesCommand, Result<SaleDto>>(
                request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SaleDto>.Success(resultDto));

        _mapperMock
            .Setup(m => m.Map<CreateSalesResponse>(resultDto))
            .Returns(mappedResponse);

        var result = await _controller.CreateSale(request, CancellationToken.None);

        var createdResult = result as CreatedResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);

        var response = createdResult.Value as ApiResponseWithData<CreateSalesResponse>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data.Should().BeEquivalentTo(mappedResponse);
    }

    [Fact]
    public async Task CreateSale_Should_Return_InternalServerError_When_Exception_Occurs()
    {
        var request = new CreateSalesRequest
        {
            CustomerId = Guid.NewGuid(),
            BranchName = "Loja 1",
            SoldAt = DateTime.UtcNow,
            Items = []
        };

        _dispatcherMock
            .Setup(d => d.SendValidatedAsync<CreateSalesRequest, CreateSalesCommand, Result<SaleDto>>(
                It.IsAny<CreateSalesRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Simulated failure"));

        var result = await _controller.CreateSale(request, CancellationToken.None);

        var errorResult = result as ObjectResult;
        errorResult.Should().NotBeNull();
        errorResult!.StatusCode.Should().Be(500);

        var response = errorResult.Value as ApiResponse;
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Message.Should().Be("An unexpected error occurred.");
    }

    [Fact]
    public async Task CreateSale_Should_Return_BadRequest_When_Dispatcher_Returns_Failure()
    {
        var request = new CreateSalesRequest
        {
            CustomerId = Guid.NewGuid(),
            BranchName = "Loja Central",
            SoldAt = DateTime.UtcNow,
            Items =
            [
                new() { ProductId = Guid.NewGuid(), Quantity = 2 }
            ]
        };

        var failureResult = Result<SaleDto>.BusinessFailure("Error occurred");

        _dispatcherMock
            .Setup(d => d.SendValidatedAsync<CreateSalesRequest, CreateSalesCommand, Result<SaleDto>>(
                It.IsAny<CreateSalesRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _controller.CreateSale(request, CancellationToken.None);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);

        var response = badRequest.Value as ApiResponse;
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Message.Should().Be("BusinessRuleViolation");
    }

    [Fact]
    public async Task GetAllSales_Should_Return_Sales_When_Successful()
    {
        var request = new GetAllSalesPaginationRequest { Page = 1, PageSize = 10, Search = "" };
        var dtoList = new PaginatedList<SaleDto>
        {
            Items = [],
            Page = 0,
            PageSize = 10,
            TotalCount = 1
        };

        _dispatcherMock
            .Setup(x => x.SendValidatedAsync<GetAllSalesPaginationRequest, GetAllSalesQuery, Result<PaginatedList<SaleDto>>>(
                request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaginatedList<SaleDto>>.Success(dtoList));

        _mapperMock
            .Setup(m => m.Map<List<SalesResponse>>(dtoList.Items))
            .Returns([]);

        var result = await _controller.GetAllSales(request, CancellationToken.None);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetSaleById_Should_Return_Sale_When_Successful()
    {
        var saleId = Guid.NewGuid();
        var dto = new SaleDto { Id = saleId };

        _dispatcherMock
            .Setup(d => d.SendValidatedAsync<GetSaleByIdRequest, GetSaleByIdQuery, Result<SaleDto>>(
                It.Is<GetSaleByIdRequest>(r => r.Id == saleId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SaleDto>.Success(dto));

        _mapperMock
            .Setup(m => m.Map<SalesResponse>(dto))
            .Returns(new SalesResponse { Id = saleId });

        var result = await _controller.GetSaleById(saleId, CancellationToken.None);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as ApiResponseWithData<SalesResponse>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data!.Id.Should().Be(saleId);
    }

    [Fact]
    public async Task CancelSale_Should_Return_Ok_When_Successful()
    {
        var saleId = Guid.NewGuid();

        _dispatcherMock
            .Setup(d => d.SendValidatedAsync<CancelSaleRequest, CancelSaleCommand, Result<Guid>>(
                It.Is<CancelSaleRequest>(r => r.SaleId == saleId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(saleId));

        var result = await _controller.CancelSale(saleId, CancellationToken.None);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as ApiResponse;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Message.Should().Be("Sale cancelled successfully");
    }

    [Fact]
    public async Task CancelItem_Should_Return_Ok_When_Successful()
    {
        var saleId = Guid.NewGuid();
        var itemIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        var resultDto = new CancelSaleDto
        {
            SaleId = saleId,
            CancelledItems =
        [
            new()
            {
                ItemId = itemIds[0],
                ProductId = Guid.NewGuid(),
                ProductName = "Produto A",
                Quantity = 2,
                FinalUnitPrice = 10.00m,
                TotalAmount = 20.00m,
                Status = "Cancelled"
            },
            new()
            {
                ItemId = itemIds[1],
                ProductId = Guid.NewGuid(),
                ProductName = "Produto B",
                Quantity = 1,
                FinalUnitPrice = 15.50m,
                TotalAmount = 15.50m,
                Status = "Cancelled"
            }
        ]
        };

        _dispatcherMock
            .Setup(d => d.SendValidatedAsync<CancelSaleItemsRequest, CancelSaleItemsCommand, Result<CancelSaleDto>>(
                It.IsAny<CancelSaleItemsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CancelSaleDto>.Success(resultDto));

        var result = await _controller.CancelItem(saleId, itemIds, CancellationToken.None);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as ApiResponseWithData<CancelSaleDto>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data.Should().BeEquivalentTo(resultDto);
    }

    [Fact]
    public async Task GetAllSales_Should_Return_BadRequest_When_Dispatcher_Returns_Failure()
    {
        var request = new GetAllSalesPaginationRequest { Page = 1, PageSize = 10 };
        var failureResult = Result<PaginatedList<SaleDto>>.BusinessFailure("Erro de validação");

        _dispatcherMock
            .Setup(d => d.SendValidatedAsync<GetAllSalesPaginationRequest, GetAllSalesQuery, Result<PaginatedList<SaleDto>>>(
                request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _controller.GetAllSales(request, CancellationToken.None);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);

        var response = badRequest.Value as ApiResponse;
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Message.Should().Be("BusinessRuleViolation");
    }

    [Fact]
    public async Task GetSaleById_Should_Return_BadRequest_When_Dispatcher_Returns_Failure()
    {
        var saleId = Guid.NewGuid();
        var failureResult = Result<SaleDto>.BusinessFailure("Erro ao buscar venda");

        _dispatcherMock
            .Setup(d => d.SendValidatedAsync<GetSaleByIdRequest, GetSaleByIdQuery, Result<SaleDto>>(
                It.IsAny<GetSaleByIdRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _controller.GetSaleById(saleId, CancellationToken.None);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);

        var response = badRequest.Value as ApiResponse;
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Message.Should().Be("BusinessRuleViolation");
    }

    [Fact]
    public async Task CancelSale_Should_Return_BadRequest_When_Dispatcher_Returns_Failure()
    {
        var saleId = Guid.NewGuid();
        var failureResult = Result<Guid>.BusinessFailure("Falha ao cancelar");

        _dispatcherMock
            .Setup(d => d.SendValidatedAsync<CancelSaleRequest, CancelSaleCommand, Result<Guid>>(
                It.IsAny<CancelSaleRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _controller.CancelSale(saleId, CancellationToken.None);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);

        var response = badRequest.Value as ApiResponse;
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Message.Should().Be("BusinessRuleViolation");
    }

    [Fact]
    public async Task CancelItem_Should_Return_BadRequest_When_Dispatcher_Returns_Failure()
    {
        var saleId = Guid.NewGuid();
        var itemIds = new List<Guid> { Guid.NewGuid() };
        var failureResult = Result<CancelSaleDto>.BusinessFailure("Erro ao cancelar itens");

        _dispatcherMock
            .Setup(d => d.SendValidatedAsync<CancelSaleItemsRequest, CancelSaleItemsCommand, Result<CancelSaleDto>>(
                It.IsAny<CancelSaleItemsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _controller.CancelItem(saleId, itemIds, CancellationToken.None);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);

        var response = badRequest.Value as ApiResponse;
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Message.Should().Be("BusinessRuleViolation");
    }
}