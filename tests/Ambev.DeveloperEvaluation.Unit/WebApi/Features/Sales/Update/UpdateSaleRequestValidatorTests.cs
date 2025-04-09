using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Update;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales.Update;

public class UpdateSaleRequestValidatorTests
{
    private readonly UpdateSaleRequestValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_SaleId_Is_Empty()
    {
        var request = new UpdateSaleRequest
        {
            SaleId = Guid.Empty
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SaleId" && e.ErrorMessage == "Id is required.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_SaleId_Is_Valid()
    {
        var request = new UpdateSaleRequest
        {
            SaleId = Guid.NewGuid(),
            SoldAt = DateTime.UtcNow,
            BranchName = "Filial Teste",
            CustomerId = Guid.NewGuid(),
            Items = new List<SaleItemRequest>
            {
                new SaleItemRequest
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 2
                }
            }
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }
}
