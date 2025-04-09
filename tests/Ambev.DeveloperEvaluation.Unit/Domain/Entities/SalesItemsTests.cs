using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemTests
{
    private readonly Product _product = new("Produto Teste", 100m, "Descrição", 50, ProductCategory.Electronics);

    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        var item = new SaleItem(_product, 5);

        item.Product.Should().Be(_product);
        item.ProductId.Should().Be(_product.Id);
        item.Quantity.Should().Be(5);
        item.OriginalUnitPrice.Should().Be(100);
        item.Status.Should().Be(SaleItemStatus.Created);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenQuantityIsZero()
    {
        var act = () => new SaleItem(_product, 0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ChangeQuantity_ShouldUpdateQuantity()
    {
        var item = new SaleItem(_product, 5);

        item.ChangeQuantity(10);

        item.Quantity.Should().Be(10);
        item.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void ChangeQuantity_ShouldThrow_WhenInvalid()
    {
        var item = new SaleItem(_product, 5);
        var act = () => item.ChangeQuantity(0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Cancel_ShouldUpdateStatusAndTimestamp()
    {
        var item = new SaleItem(_product, 5);
        var user = new User { Id = Guid.NewGuid(), Username = "admin" };

        item.Cancel(user);

        item.Status.Should().Be(SaleItemStatus.Cancelled);
        item.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenUserIsNull()
    {
        var item = new SaleItem(_product, 5);
        var act = () => item.Cancel(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Delete_ShouldUpdateStatusDeletedAtAndUser()
    {
        var item = new SaleItem(_product, 5);
        var user = new User { Id = Guid.NewGuid(), Username = "admin" };

        item.Delete(user);

        item.Status.Should().Be(SaleItemStatus.Deleted);
        item.DeletedAt.Should().NotBeNull();
        item.DeletedBy.Should().Be(user);
    }

    [Fact]
    public void Delete_ShouldThrow_WhenUserIsNull()
    {
        var item = new SaleItem(_product, 5);
        var act = () => item.Delete(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ApplyDiscount_ShouldApply20Percent_WhenQuantityIs10OrMore()
    {
        var item = new SaleItem(_product, 10);
        item.ApplyDiscount();

        item.DiscountPerUnit.Should().Be(20);
        item.TotalAmount.Should().Be((100 - 20) * 10);
    }

    [Fact]
    public void ApplyDiscount_ShouldApply10Percent_WhenQuantityIsBetween4And9()
    {
        var item = new SaleItem(_product, 4);
        item.ApplyDiscount();

        item.DiscountPerUnit.Should().Be(10);
        item.TotalAmount.Should().Be((100 - 10) * 4);
    }

    [Fact]
    public void ApplyDiscount_ShouldNotApplyDiscount_WhenQuantityIsLessThan4()
    {
        var item = new SaleItem(_product, 2);
        item.ApplyDiscount();

        item.DiscountPerUnit.Should().Be(0);
        item.TotalAmount.Should().Be(100 * 2);
    }

    [Fact]
    public void ApplyDiscount_ShouldThrow_WhenQuantityExceeds20()
    {
        var item = new SaleItem(_product, 21);
        var act = () => item.ApplyDiscount();
        act.Should().Throw<DomainException>()
            .WithMessage("It's not allowed to sell more than 20 identical items.");
    }
}
