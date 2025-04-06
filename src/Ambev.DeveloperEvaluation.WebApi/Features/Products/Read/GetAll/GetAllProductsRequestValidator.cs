using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Read.GetAll;

/// <summary>
/// Validator for GetAllProductsPaginationRequest.
/// Ensures that the Page number is greater than 0,
/// the PageSize is greater than 0 and less than or equal to 100,
/// and the SearchCategory is 30 characters or less.
/// </summary>
public class GetAllProductsRequestValidator : AbstractValidator<GetAllProductsPaginationRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllProductsRequestValidator"/> class.
    /// Defines validation rules for GetAllProductsPaginationRequest.
    /// </summary>
    public GetAllProductsRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must be 100 or less.");

        RuleFor(x => x.Search)
            .MaximumLength(50).WithMessage("Search category must be 50 characters or less.");
    }
}