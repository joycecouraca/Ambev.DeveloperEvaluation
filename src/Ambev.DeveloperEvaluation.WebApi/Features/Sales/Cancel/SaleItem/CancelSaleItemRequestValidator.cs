using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Cancel.SaleItem;

public class CancelSaleItemsRequestValidator : AbstractValidator<CancelSaleItemsRequest>
{
    public CancelSaleItemsRequestValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty();
        RuleFor(x => x.ItemIds)
            .NotEmpty()
            .WithMessage("At least one item must be selected for cancellation.");
    }
}
