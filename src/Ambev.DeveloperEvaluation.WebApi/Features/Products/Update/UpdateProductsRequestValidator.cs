using Ambev.DeveloperEvaluation.WebApi.Features.Products.Create;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Update;

/// <summary>
/// Validator for UpdateProductsRequest to ensure all required fields are properly filled and valid.
/// </summary>
public class UpdateProductsRequestValidator : AbstractValidator<UpdateProductsRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProductsRequestValidator"/> class.
    /// Sets up validation rules for the UpdateProductsRequest to ensure all required fields are properly filled and valid.
    /// </summary>
    public UpdateProductsRequestValidator()
    {
        Include(new CreateProductsRequestValidator());

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
