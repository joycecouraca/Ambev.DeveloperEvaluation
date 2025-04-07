using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public virtual Sale Sale { get; set; } = default!;

    public Guid ProductId { get; private set; }
    public virtual Product Product { get; private set; } = null!;

    public int Quantity { get; private set; }
    public decimal OriginalUnitPrice { get; private set; }
    public decimal DiscountPerUnit { get; private set; }
    public decimal FinalUnitPrice => OriginalUnitPrice - DiscountPerUnit;
    public decimal TotalAmount => FinalUnitPrice * Quantity;
        
    public SaleItemStatus Status { get; private set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public virtual User? DeletedBy { get; private set; }
    public virtual User? CancelBy { get; private set; }

    private SaleItem() { }

    public SaleItem(Product product, int quantity)
    {
        ArgumentNullException.ThrowIfNull(product);

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        Product = product;
        ProductId = product.Id;
        Quantity = quantity;
        OriginalUnitPrice = product.Price;
        Status = SaleItemStatus.Created;
    }

    public void ChangeQuantity(int newQuantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(newQuantity);

        Quantity = newQuantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(User cancelledBy)
    {
        ArgumentNullException.ThrowIfNull(cancelledBy);

        Status = SaleItemStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete(User deletedBy)
    {
        ArgumentNullException.ThrowIfNull(deletedBy);

        Status = SaleItemStatus.Deleted;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void ApplyDiscount()
    {
        CheckQuantityLimit();

        decimal discountRate = 0;

        if (Quantity >= 10)
            discountRate = 0.20m;
        else if (Quantity >= 4)
            discountRate = 0.10m;

        DiscountPerUnit = OriginalUnitPrice * discountRate;
    }

    private void CheckQuantityLimit()
    {
        if (Quantity > 20)
            throw new DomainException("It's not allowed to sell more than 20 identical items.");
    }
}
