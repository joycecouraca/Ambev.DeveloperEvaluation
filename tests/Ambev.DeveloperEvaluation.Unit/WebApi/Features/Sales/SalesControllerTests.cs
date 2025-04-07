using Ambev.DeveloperEvaluation.Application.Common.Abstractions;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create.Dtos;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Common;

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

        var resultDto = new CreateSaleDto
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
            .Setup(x => x.SendValidatedAsync<CreateSalesRequest, CreateSalesCommand, Result<CreateSaleDto>>(
                request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CreateSaleDto>.Success(resultDto));

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
            .Setup(d => d.SendValidatedAsync<CreateSalesRequest, CreateSalesCommand, Result<CreateSaleDto>>(
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

        var failureResult = Result<CreateSaleDto>.Failure("Error occurred");

        _dispatcherMock
            .Setup(d => d.SendValidatedAsync<CreateSalesRequest, CreateSalesCommand, Result<CreateSaleDto>>(
                It.IsAny<CreateSalesRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _controller.CreateSale(request, CancellationToken.None);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);

        var response = badRequest.Value as ApiResponse;
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Message.Should().Be("Business rule violation.");
    }
}