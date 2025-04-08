using Ambev.DeveloperEvaluation.WebApi.Features.Products.Create;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Update;

public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {
        Include(new CreateSalesRequestValidator());

        RuleFor(x => x.SaleId)
           .NotEmpty().WithMessage("Id is required.");
    }
}
