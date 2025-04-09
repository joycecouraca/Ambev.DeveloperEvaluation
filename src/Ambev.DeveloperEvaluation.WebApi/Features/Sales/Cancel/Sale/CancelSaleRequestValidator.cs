using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Cancel.Sale;

public class CancelSaleRequestValidator : AbstractValidator<CancelSaleRequest>
{
    public CancelSaleRequestValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty()
            .WithMessage("SaleId is required.");
    }
}
