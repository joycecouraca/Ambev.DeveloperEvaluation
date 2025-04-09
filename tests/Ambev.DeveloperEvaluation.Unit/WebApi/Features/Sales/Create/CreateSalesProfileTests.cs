using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales.Create;

public class CreateSalesProfileTests
{
    private readonly IMapper _mapper;

    public CreateSalesProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CreateSalesProfile>());

        config.AssertConfigurationIsValid();

        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Should_Map_CreateSalesRequest_To_CreateSalesCommand()
    {
        var request = new CreateSalesRequest
        {
            CustomerId = Guid.NewGuid(),
            SoldAt = DateTime.UtcNow,
            BranchName = "São Paulo",
            Items =
            [
                new() {
                    ProductId = Guid.NewGuid(),
                    Quantity = 5
                }
            ]
        };

        var result = _mapper.Map<CreateSalesCommand>(request);

        result.Should().NotBeNull();
        result.CustomerId.Should().Be(request.CustomerId);
        result.SoldAt.Should().Be(request.SoldAt);
        result.BranchName.Should().Be(request.BranchName);
        result.Items.Should().HaveCount(1);
        result.Items[0].ProductId.Should().Be(request.Items[0].ProductId);
        result.Items[0].Quantity.Should().Be(request.Items[0].Quantity);
    }

    [Fact]
    public void Should_Map_CreateSaleDto_To_CreateSalesResponse()
    {
        var dto = new SaleDto
        {
            SaleNumber = 123456,
            BranchName = "Rio de Janeiro",
            SoldAt = DateTime.UtcNow,
            Status = "Created",
            TotalSaleAmount = 500,
            Items =
            [
                new() {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto A",
                    Quantity = 3,
                    UnitPrice = 100,
                    TotalAmount = 300,
                    DiscountPerUnit = 0
                }
            ]
        };

        var response = _mapper.Map<CreateSalesResponse>(dto);

        response.Should().NotBeNull();
        response.SaleNumber.Should().Be(dto.SaleNumber);
        response.BranchName.Should().Be(dto.BranchName);
        response.Status.Should().Be(dto.Status);
        response.TotalSaleAmount.Should().Be(dto.TotalSaleAmount);
        response.Items.Should().HaveCount(1);
        response.Items[0].ProductId.Should().Be(dto.Items[0].ProductId);
    }
}