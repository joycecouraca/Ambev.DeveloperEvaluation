using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common.Response;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales.Create;

public class CreateSalesResponseTests
{
    [Fact]
    public void Should_Create_Response_With_Valid_Values()
    {
        var response = new CreateSalesResponse
        {
            Id = Guid.NewGuid(),
            SaleNumber = 123456789,
            SoldAt = DateTime.UtcNow,
            TotalSaleAmount = 150.50m,
            BranchName = "Central",
            CustomerName = "João da Silva",
            Status = "Completed",
            Items =
            [
                new SaleItemResponse
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto Teste",
                    Quantity = 2,
                    UnitPrice = 50.25m,
                    TotalAmount = 100.50m,
                    DiscountPerUnit = 0
                }
            ]
        };

        response.Id.Should().NotBeEmpty();
        response.SaleNumber.Should().Be(123456789);
        response.SoldAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        response.TotalSaleAmount.Should().Be(150.50m);
        response.BranchName.Should().Be("Central");
        response.CustomerName.Should().Be("João da Silva");
        response.Status.Should().Be("Completed");
        response.Items.Should().HaveCount(1);

        var item = response.Items[0];
        item.ProductName.Should().Be("Produto Teste");
        item.Quantity.Should().Be(2);
        item.UnitPrice.Should().Be(50.25m);
        item.TotalAmount.Should().Be(100.50m);
        item.DiscountTotal.Should().Be(0);
        item.DiscountPerUnit.Should().Be(0);
    }
}
