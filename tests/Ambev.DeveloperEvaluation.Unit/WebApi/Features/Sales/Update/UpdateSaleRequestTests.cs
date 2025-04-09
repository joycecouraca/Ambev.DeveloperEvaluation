using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Update;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales.Update;

public class UpdateSaleRequestTests
{
    [Fact]
    public void Constructor_Should_Set_Properties_Correctly()
    {
        var saleId = Guid.NewGuid();
        var soldAt = DateTime.UtcNow;
        var branchName = "Filial Teste";
        var items = new List<SaleItemRequest>
        {
            new SaleItemRequest
            {
                ProductId = Guid.NewGuid(),
                Quantity = 2
            }
        };

        var request = new UpdateSaleRequest
        {
            SaleId = saleId,
            SoldAt = soldAt,
            BranchName = branchName,
            Items = items
        };

        request.SaleId.Should().Be(saleId);
        request.SoldAt.Should().BeCloseTo(soldAt, TimeSpan.FromSeconds(1));
        request.BranchName.Should().Be(branchName);
        request.Items.Should().BeEquivalentTo(items);
    }
}
