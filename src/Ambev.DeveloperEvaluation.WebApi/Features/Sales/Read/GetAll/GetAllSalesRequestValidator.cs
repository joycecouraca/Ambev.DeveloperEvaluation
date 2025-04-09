using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Read.GetAll;

public class GetAllSalesRequestValidator : AbstractValidator<GetAllSalesPaginationRequest>
{
    public GetAllSalesRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");
    }
}
