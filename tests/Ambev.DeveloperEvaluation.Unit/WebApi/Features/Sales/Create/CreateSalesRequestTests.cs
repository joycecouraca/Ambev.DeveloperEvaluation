using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales.Create;

public class CreateSalesRequestTests
{
    [Fact]
    public void Should_Initialize_CreateSalesRequest_With_Valid_Data()
    {
        var soldAt = DateTime.UtcNow;
        const string branchName = "Unidade Centro";
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var request = new CreateSalesRequest
        {
            SoldAt = soldAt,
            BranchName = branchName,
            CustomerId = customerId,
            Items =
            [
                new SaleItemRequest
                {
                    ProductId = productId,
                    Quantity = 5
                }
            ]
        };

        request.SoldAt.Should().BeCloseTo(soldAt, TimeSpan.FromSeconds(1));
        request.BranchName.Should().Be(branchName);
        request.CustomerId.Should().Be(customerId);
        request.Items.Should().HaveCount(1);

        var item = request.Items[0];
        item.ProductId.Should().Be(productId);
        item.Quantity.Should().Be(5);
    }
}
