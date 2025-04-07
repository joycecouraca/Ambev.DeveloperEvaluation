using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
        Status = SaleStatus.Created;
    }

    public required long SaleNumber { get; init; }
    public string BranchName { get; private set; } = default!;
    public DateTime SoldAt { get; set; }
    public decimal TotalSaleAmount { get; private set; }
    public decimal TotalDiscount { get; private set; }
    public SaleStatus Status { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid BoughtById { get; private set; }
    public required User BoughtBy { get; set; }

    public Guid CreatedById { get; private set; }
    public required User CreatedBy { get; set; }
    public Guid CancelById { get; private set; }
    public User? CancelledBy { get; private set; }
    public Guid DeleteById { get; private set; }
    public User? DeletedBy { get; private set; }

    public ICollection<SaleItem> Items { get; private set; } = [];

    [NotMapped]
    public IEnumerable<SaleItem> ActiveItems =>
        Items.Where(i => i.Status != SaleItemStatus.Deleted);

    public bool CanBeCancelled => Status != SaleStatus.Cancelled;
    public bool CanBeEdited => Status == SaleStatus.Created;
    public bool HasAnyDeletedItem => Items.Any(i => i.Status == SaleItemStatus.Deleted);

    public static Sale Create(User customer, User creator, DateTime soldAt, string branchName)
    {
        ArgumentNullException.ThrowIfNull(customer);
        ArgumentNullException.ThrowIfNull(creator);
        ArgumentException.ThrowIfNullOrWhiteSpace(branchName);

        return new Sale
        {
            BoughtBy = customer,
            BoughtById = customer.Id,
            CreatedBy = creator,
            CreatedById = creator.Id,
            SoldAt = soldAt,
            BranchName = branchName,
            SaleNumber = DateTime.UtcNow.Ticks,
        };
    }

    public void AddItems(params SaleItem[] items)
    {
        ArgumentNullException.ThrowIfNull(items);

        foreach (var item in items)
        {
            item.Product.DecreaseQuantity(item.Quantity);
            Items.Add(item);
        }

        RefreshTotalAmount();
    }

    public void Cancel(User cancelledBy)
    {
        ArgumentNullException.ThrowIfNull(cancelledBy);

        foreach (var item in Items)
        {
            item.Cancel(cancelledBy);
        }

        Status = SaleStatus.Cancelled;
        CancelledAt = UpdatedAt = DateTime.UtcNow;
        CancelledBy = cancelledBy;
    }

    public void Change(User customer, DateTime soldAt, string branchName)
    {
        ArgumentNullException.ThrowIfNull(customer);

        BoughtBy = customer;
        BoughtById = customer.Id;
        SoldAt = soldAt;
        BranchName = branchName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete(User deletedBy)
    {
        ArgumentNullException.ThrowIfNull(deletedBy);

        Cancel(deletedBy);
        Status = SaleStatus.Deleted;
        DeletedAt = UpdatedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void RefreshTotalAmount()
    {
        TotalSaleAmount = Items
            .Where(i => i.Status == SaleItemStatus.Created)
            .Sum(i => i.TotalAmount);

        TotalDiscount = Items
            .Where(i => i.Status == SaleItemStatus.Created)
            .Sum(i => i.DiscountPerUnit);
    }

    public void DeleteItems(User deletedBy, params SaleItem[] items)
    {
        ArgumentNullException.ThrowIfNull(deletedBy);
        ArgumentNullException.ThrowIfNull(items);

        foreach (var item in items)
        {
            item.Delete(deletedBy);
        }

        RefreshTotalAmount();
    }

    public void ChangeQuantities(Dictionary<Guid, int> productQuantities)
    {
        ArgumentNullException.ThrowIfNull(productQuantities);

        foreach (var item in Items)
        {
            if (item.Status == SaleItemStatus.Created &&
                productQuantities.TryGetValue(item.ProductId, out var newQuantity))
            {
                item.ChangeQuantity(newQuantity);
            }
        }

        RefreshTotalAmount();
    }
}
