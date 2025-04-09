using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    private User CreateUser(string name = "User") => new() { Id = Guid.NewGuid(), Username = name };

    private Product CreateProduct(string name = "Product", int quantity = 10) =>
        new(name, 100, "Description", quantity, ProductCategory.Others);

    [Fact]
    public void Create_ShouldInitializeSaleCorrectly()
    {
        var customer = CreateUser("Customer");
        var creator = CreateUser("Creator");
        var soldAt = DateTime.UtcNow;
        var branchName = "Branch";

        var sale = Sale.Create(customer, creator, soldAt, branchName);

        sale.BoughtBy.Should().Be(customer);
        sale.CreatedBy.Should().Be(creator);
        sale.SoldAt.Should().Be(soldAt);
        sale.BranchName.Should().Be(branchName);
        sale.Status.Should().Be(SaleStatus.Created);
        sale.Items.Should().BeEmpty();
        sale.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void AddItems_ShouldAddItemsAndRefreshTotals()
    {
        var customer = CreateUser();
        var creator = CreateUser();
        var product = CreateProduct();
        var item = new SaleItem(product, 2);
        var sale = Sale.Create(customer, creator, DateTime.UtcNow, "Branch");

        sale.AddItems(item);

        sale.Items.Should().ContainSingle();
        sale.TotalSaleAmount.Should().Be(item.TotalAmount);
        sale.TotalDiscount.Should().Be(item.DiscountPerUnit);
    }

    [Fact]
    public void Cancel_ShouldUpdateStatusAndAllItems()
    {
        var user = CreateUser();
        var sale = Sale.Create(user, user, DateTime.UtcNow, "Branch");
        var item = new SaleItem(CreateProduct(), 1);
        sale.AddItems(item);

        sale.Cancel(user);

        sale.Status.Should().Be(SaleStatus.Cancelled);
        item.Status.Should().Be(SaleItemStatus.Cancelled);
        sale.CancelledBy.Should().Be(user);
        sale.CancelledAt.Should().NotBeNull();
    }

    [Fact]
    public void Change_ShouldUpdateSaleInfo()
    {
        var oldCustomer = CreateUser("Old");
        var newCustomer = CreateUser("New");
        var sale = Sale.Create(oldCustomer, oldCustomer, DateTime.UtcNow, "OldBranch");
        var soldAt = DateTime.UtcNow.AddDays(-1);

        sale.Change(newCustomer, soldAt, "NewBranch");

        sale.BoughtBy.Should().Be(newCustomer);
        sale.SoldAt.Should().Be(soldAt);
        sale.BranchName.Should().Be("NewBranch");
    }

    [Fact]
    public void Delete_ShouldCancelAndMarkAsDeleted()
    {
        var user = CreateUser();
        var sale = Sale.Create(user, user, DateTime.UtcNow, "Branch");
        sale.AddItems(new SaleItem(CreateProduct(), 1));

        sale.Delete(user);

        sale.Status.Should().Be(SaleStatus.Deleted);
        sale.DeletedBy.Should().Be(user);
        sale.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public void RefreshTotalAmount_ShouldSumOnlyCreatedItems()
    {
        var sale = Sale.Create(CreateUser(), CreateUser(), DateTime.UtcNow, "Branch");
        var product = CreateProduct();
        var item1 = new SaleItem(product, 1);
        var item2 = new SaleItem(product, 1);
        sale.AddItems(item1, item2);
        item2.Cancel(CreateUser());

        sale.RefreshTotalAmount();

        sale.TotalSaleAmount.Should().Be(item1.TotalAmount);
    }

    [Fact]
    public void DeleteItems_ShouldMarkItemsAsDeleted()
    {
        var user = CreateUser();
        var sale = Sale.Create(user, user, DateTime.UtcNow, "Branch");
        var item = new SaleItem(CreateProduct(), 1);
        sale.AddItems(item);

        sale.DeleteItems(user, item);

        item.Status.Should().Be(SaleItemStatus.Deleted);
        sale.TotalSaleAmount.Should().Be(0);
    }

    [Fact]
    public void ChangeQuantities_ShouldUpdateQuantities()
    {
        var product = CreateProduct();
        var item = new SaleItem(product, 1);
        var sale = Sale.Create(CreateUser(), CreateUser(), DateTime.UtcNow, "Branch");
        sale.AddItems(item);

        sale.ChangeQuantities(new Dictionary<Guid, int> { { item.ProductId, 3 } });

        item.Quantity.Should().Be(3);
    }

    [Fact]
    public void CancelItems_ShouldOnlyCancelCreatedItems()
    {
        var user = CreateUser();
        var product = CreateProduct();
        var item1 = new SaleItem(product, 1);
        var item2 = new SaleItem(product, 1);
        item2.Cancel(user);

        var sale = Sale.Create(user, user, DateTime.UtcNow, "Branch");
        sale.AddItems(item1, item2);

        sale.CancelItems(user, item1, item2);

        item1.Status.Should().Be(SaleItemStatus.Cancelled);
        item2.Status.Should().Be(SaleItemStatus.Cancelled);
    }

    [Fact]
    public void CancelItems_ShouldCancelSaleIfAllItemsAreCancelledOrDeleted()
    {
        var user = CreateUser();
        var sale = Sale.Create(user, user, DateTime.UtcNow, "Branch");
        var item1 = new SaleItem(CreateProduct(), 1);
        var item2 = new SaleItem(CreateProduct(), 1);
        sale.AddItems(item1, item2);
        item2.Delete(user);

        sale.CancelItems(user, item1);

        sale.Status.Should().Be(SaleStatus.Cancelled);
    }
}
