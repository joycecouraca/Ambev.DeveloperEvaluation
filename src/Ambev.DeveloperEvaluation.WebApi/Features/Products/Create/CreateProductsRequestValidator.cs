using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Create;

/// <summary>
/// Validator for CreateProductsRequest to ensure all required fields are properly filled and valid.
/// </summary>
public class CreateProductsRequestValidator : AbstractValidator<CreateProductsRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProductsRequestValidator"/> class.
    /// Sets up validation rules for the CreateProductsRequest to ensure all required fields are properly filled and valid.
    /// </summary>
    public CreateProductsRequestValidator()
    {
        RuleFor(product => product.Name).NotEmpty().Length(3, 100);
        RuleFor(product => product.Description).NotEmpty().Length(3, 300);
        RuleFor(product => product.Price).GreaterThan(0);
        RuleFor(product => product.Category)
            .NotEmpty()
            .Must(category => Enum.IsDefined(typeof(ProductCategory), category))
            .WithMessage("Category must be a valid ProductCategory name.");
        RuleFor(product => product.Quantity).GreaterThan(0);
    }
}
