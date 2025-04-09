using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Read.GetById;

public class GetProductByIdValidator : AbstractValidator<GetProductByIdRequest>
{
    public GetProductByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Product ID must not be empty.");
    }
}