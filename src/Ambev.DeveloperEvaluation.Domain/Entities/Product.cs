using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Serilog;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = null!;
    public decimal Price { get; private set; }
    public string Description { get; private set; } = null!;
    public int Quantity { get; private set; }
    public ProductCategory Category { get; private set; } = default!;
    public bool Active { get; private set; } = true;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    private Product() { } 

    public Product(string name, decimal price, string description, int quantity, ProductCategory category)
    {
        SetBasicInfo(name, price, description, category);

        if (quantity < 0)
            throw new DomainException("Product quantity cannot be negative.");

        Quantity = quantity;
    }

    public void Change(string name, decimal price, string description, ProductCategory category)
    {
        SetBasicInfo(name, price, description, category);
        MarkAsUpdated();
    }

    public void DecreaseQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        if (Quantity - quantity < 0)
        {
            Log.Error("Product#{Id} '{Name}' - insufficient stock. Current: {Quantity}, Attempted Decrease: {quantity}", Id, Name, Quantity, quantity);
            throw new DomainException($"Insufficient stock for product '{Name}'.");
        }

        Quantity -= quantity;
        MarkAsUpdated();
    }

    public void Disable()
    {
        Active = false;
        MarkAsUpdated();
    }

    public void Enable()
    {
        Active = true;
        MarkAsUpdated();
    }

    private void SetBasicInfo(string name, decimal price, string description, ProductCategory category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be empty.");

        if (price <= 0)
            throw new DomainException("Product price must be greater than zero.");

        Name = name;
        Price = price;
        Description = description;
        Category = category;
    }

    private void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}