using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Cancel.SaleItem;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales.Cancel;

public class CancelSaleItemsRequestValidatorTests
{
    private readonly CancelSaleItemsRequestValidator _validator;

    public CancelSaleItemsRequestValidatorTests()
    {
        _validator = new CancelSaleItemsRequestValidator();
    }

    [Fact]
    public void Should_Have_Error_When_SaleId_Is_Empty()
    {
        var model = new CancelSaleItemsRequest(Guid.Empty, new List<Guid> { Guid.NewGuid() });

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.SaleId);
    }

    [Fact]
    public void Should_Have_Error_When_ItemIds_Is_Empty()
    {
        var model = new CancelSaleItemsRequest(Guid.NewGuid(), new List<Guid>());

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ItemIds)
              .WithErrorMessage("At least one item must be selected for cancellation.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        var model = new CancelSaleItemsRequest(Guid.NewGuid(), new List<Guid> { Guid.NewGuid() });

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.SaleId);
        result.ShouldNotHaveValidationErrorFor(x => x.ItemIds);
    }
}
