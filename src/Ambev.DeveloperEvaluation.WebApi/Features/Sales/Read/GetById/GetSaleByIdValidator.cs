using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Read.GetById;

public class GetSaleByIdValidator : AbstractValidator<GetSaleByIdRequest>
{
    public GetSaleByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Sale ID must be provided.");
    }
}
