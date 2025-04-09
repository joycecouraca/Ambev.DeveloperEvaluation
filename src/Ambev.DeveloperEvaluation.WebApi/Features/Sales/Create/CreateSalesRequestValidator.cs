using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;

public class CreateSalesRequestValidator : AbstractValidator<CreateSalesRequest>
{
    public CreateSalesRequestValidator()
    {
        RuleFor(x => x.SoldAt)
            .NotEmpty()
            .WithMessage("SoldAt must be provided.");

        RuleFor(x => x.BranchName)
            .NotEmpty()
            .WithMessage("Branch name is required.")
            .MaximumLength(50)
            .WithMessage("Branch name must be at most 50 characters.");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("CustomerId is required.");

        RuleFor(x => x.Items)
            .NotNull()
            .WithMessage("At least one sale item must be provided.")
            .Must(items => items.Count != 0)
            .WithMessage("At least one sale item must be provided.");

        RuleForEach(x => x.Items).SetValidator(new SaleItemRequestValidator());
    }
}

public class SaleItemRequestValidator : AbstractValidator<SaleItemRequest>
{
    public SaleItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.")
            .LessThanOrEqualTo(20)
            .WithMessage("You cannot sell more than 20 items of the same product.");
    }
}
