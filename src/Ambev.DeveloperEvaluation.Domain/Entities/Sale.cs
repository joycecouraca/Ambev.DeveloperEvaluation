using Ambev.DeveloperEvaluation.Domain.Abstractions;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
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

    public bool CanBeCancelled => Status == SaleStatus.Cancelled;
    public bool CanBeEdited => Status == SaleStatus.Created;
    public bool HasAnyDeletedItem => Items.Any(i => i.Status == SaleItemStatus.Deleted);

    private void AddEvent(IDomainEvent @event) => _events.Add(@event);

    public static Sale Create(User customer, User creator, DateTime soldAt, string branchName)
    {
        ArgumentNullException.ThrowIfNull(customer);
        ArgumentNullException.ThrowIfNull(creator);
        ArgumentException.ThrowIfNullOrWhiteSpace(branchName);

        var sale = new Sale
        {
            BoughtBy = customer,
            BoughtById = customer.Id,
            CreatedBy = creator,
            CreatedById = creator.Id,
            SoldAt = soldAt,
            BranchName = branchName,
            SaleNumber = DateTime.UtcNow.Ticks,
        };

        sale.AddEvent(new SaleCreatedDomainEvent(sale.Id, soldAt));

        return sale;
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

        AddEvent(new SaleCancelledDomainEvent(Id, DateTime.UtcNow));

    }

    public void Change(User customer, DateTime soldAt, string branchName)
    {
        ArgumentNullException.ThrowIfNull(customer);

        BoughtBy = customer;
        BoughtById = customer.Id;
        SoldAt = soldAt;
        BranchName = branchName;
        UpdatedAt = DateTime.UtcNow;

        AddEvent(new SaleModifiedDomainEvent(Id, DateTime.UtcNow));

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

    public void CancelItems(User cancelledBy, params SaleItem[] itemsToCancel)
    {
        ArgumentNullException.ThrowIfNull(cancelledBy);
        ArgumentNullException.ThrowIfNull(itemsToCancel);

        foreach (var item in itemsToCancel)
        {
            if (item.Status == SaleItemStatus.Created)
            {
                item.Cancel(cancelledBy);
            }
        }

        RefreshTotalAmount();

        if (Items.All(i => i.Status == SaleItemStatus.Cancelled || i.Status == SaleItemStatus.Deleted))
        {
            Status = SaleStatus.Cancelled;
            CancelledAt = UpdatedAt = DateTime.UtcNow;
            CancelledBy = cancelledBy;

            AddEvent(new ItemCancelledDomainEvent(Id, [.. itemsToCancel.Select(i => i.Id)], DateTime.UtcNow));
        }
    }
}
