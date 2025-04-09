using Ambev.DeveloperEvaluation.Application.Sales.Commands.Update;
using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common.Response;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Update;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales.Update;

public class UpdateSaleProfileTests
{
    private readonly IMapper _mapper;

    public UpdateSaleProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UpdateSaleProfile>();
        });

        _mapper = config.CreateMapper();
    }

    [Fact]
    public void UpdateSaleProfile_ConfigurationIsValid()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UpdateSaleProfile>();
        });

        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Should_Map_UpdateSaleRequest_To_UpdateSaleCommand_Correctly()
    {
        var request = new UpdateSaleRequest
        {
            SaleId = Guid.NewGuid(),
            BranchName = "Filial A",
            SoldAt = DateTime.UtcNow,
            Items = new List<SaleItemRequest>
        {
            new SaleItemRequest
            {
                ProductId = Guid.NewGuid(),
                Quantity = 2
            }
        }
        };

        var command = _mapper.Map<UpdateSaleCommand>(request);

        command.Should().NotBeNull();
        command.SaleId.Should().Be(request.SaleId);
        command.BranchName.Should().Be(request.BranchName);
        command.SoldAt.Should().Be(request.SoldAt);
        command.Items.Should().HaveCount(1);
        command.CreatedById.Should().BeEmpty(); // Ignorado no mapeamento
    }

    [Fact]
    public void Should_Map_SaleItemRequest_To_SaleItemDto_With_Ignored_Fields()
    {
        var itemRequest = new SaleItemRequest
        {
            ProductId = Guid.NewGuid(),
            Quantity = 5
        };

        var dto = _mapper.Map<SaleItemDto>(itemRequest);

        dto.Should().NotBeNull();
        dto.ProductId.Should().Be(itemRequest.ProductId);
        dto.Quantity.Should().Be(itemRequest.Quantity);

        dto.ProductName.Should().BeNull();
        dto.UnitPrice.Should().Be(0);
        dto.TotalAmount.Should().Be(0);
        dto.DiscountPerUnit.Should().Be(0);
        dto.DiscountTotal.Should().Be(0);
    }

    [Fact]
    public void Should_Map_SaleDto_To_UpdateSaleResponse()
    {
        var dto = new SaleDto
        {
            Id = Guid.NewGuid(),
            SaleNumber = 123456,
            BranchName = "Filial B",
            SoldAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CustomerName = "Fulano",
            TotalSaleAmount = 150,
            TotalDiscountAmount = 10
        };

        var response = _mapper.Map<UpdateSaleResponse>(dto);

        response.Should().NotBeNull();
        response.Id.Should().Be(dto.Id);
        response.SaleNumber.Should().Be(dto.SaleNumber);
        response.BranchName.Should().Be(dto.BranchName);
        response.SoldAt.Should().Be(dto.SoldAt);
        response.CustomerName.Should().Be(dto.CustomerName);
        response.TotalSaleAmount.Should().Be(dto.TotalSaleAmount);
        response.TotalDiscountAmount.Should().Be(dto.TotalDiscountAmount);
    }

    [Fact]
    public void Should_Map_SaleItemDto_To_SaleItemResponse_Correctly()
    {
        var dto = new SaleItemDto
        {
            ProductId = Guid.NewGuid(),
            Quantity = 3,
            ProductName = "Produto Teste",
            UnitPrice = 25.50m,
            TotalAmount = 76.50m,
            DiscountPerUnit = 1.50m            
        };

        var response = _mapper.Map<SaleItemResponse>(dto);

        response.Should().NotBeNull();
        response.ProductId.Should().Be(dto.ProductId);
        response.Quantity.Should().Be(dto.Quantity);
        response.ProductName.Should().Be(dto.ProductName);
        response.UnitPrice.Should().Be(dto.UnitPrice);
        response.TotalAmount.Should().Be(dto.TotalAmount);
        response.DiscountPerUnit.Should().Be(dto.DiscountPerUnit);
        response.DiscountTotal.Should().BeApproximately(dto.DiscountTotal, 0.01m);
    }
}
