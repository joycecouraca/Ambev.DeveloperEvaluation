using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public virtual Sales Sale { get; set; } = default!;

    public Guid ProductId { get; private set; }
    public virtual Product Product { get; private set; } = null!;

    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public decimal TotalAmount => Quantity * UnitPrice;

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

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        Product = product;
        ProductId = product.Id;
        Quantity = quantity;
        UnitPrice = product.Price;
        Status = SaleItemStatus.Created;
    }

    public void ChangeQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(newQuantity));

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
        if (Quantity >= 10 && Quantity <= 20)
        {
            UnitPrice *= 0.80m; 
        }
        else if (Quantity >= 4)
        {
            UnitPrice *= 0.90m; 
        }
        else if (Quantity > 20)
        {
            throw new DomainException("It's not allowed to sell more than 20 identical items.");
        }        
    }
}
