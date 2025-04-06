using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Read;

public class GetAllProductsRequestValidator : AbstractValidator<GetAllProductsPaginationRequest>
{
    public GetAllProductsRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must be 100 or less.");

        RuleFor(x => x.SearchCategory)
            .MaximumLength(30).WithMessage("Search category must be 30 characters or less.");
    }
}
