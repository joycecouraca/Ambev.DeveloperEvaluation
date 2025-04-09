using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Constructor_Should_Set_Properties_Correctly()
    {
        var product = new Product("Produto Teste", 100, "Descrição", 10, ProductCategory.Automotive);

        product.Name.Should().Be("Produto Teste");
        product.Price.Should().Be(100);
        product.Description.Should().Be("Descrição");
        product.Quantity.Should().Be(10);
        product.Category.Should().Be(ProductCategory.Automotive);
        product.Active.Should().BeTrue();
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_Should_Throw_When_Quantity_Is_Negative()
    {
        Action act = () => new Product("Produto", 50, "Desc", -5, ProductCategory.Others);

        act.Should().Throw<DomainException>()
            .WithMessage("Product quantity cannot be negative.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void SetBasicInfo_Should_Throw_When_Name_Is_Invalid(string? name)
    {
        Action act = () => new Product(name!, 10, "Desc", 5, ProductCategory.Electronics);

        act.Should().Throw<DomainException>()
            .WithMessage("Product name cannot be empty.");
    }

    [Fact]
    public void SetBasicInfo_Should_Throw_When_Price_Is_Zero_Or_Less()
    {
        Action act = () => new Product("Nome", 0, "Desc", 5, ProductCategory.Electronics);

        act.Should().Throw<DomainException>()
            .WithMessage("Product price must be greater than zero.");
    }

    [Fact]
    public void DecreaseQuantity_Should_Decrease_Correctly()
    {
        var product = new Product("Nome", 10, "Desc", 5, ProductCategory.Others);

        product.DecreaseQuantity(3);

        product.Quantity.Should().Be(2);
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void DecreaseQuantity_Should_Throw_When_Quantity_Is_Zero_Or_Less()
    {
        var product = new Product("Nome", 10, "Desc", 5, ProductCategory.Others);

        Action act = () => product.DecreaseQuantity(0);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Quantity must be greater than zero*");
    }

    [Fact]
    public void DecreaseQuantity_Should_Throw_When_Stock_Is_Insufficient()
    {
        var product = new Product("Nome", 10, "Desc", 2, ProductCategory.Others);

        Action act = () => product.DecreaseQuantity(3);

        act.Should().Throw<DomainException>()
            .WithMessage("Insufficient stock for product*");
    }

    [Fact]
    public void Disable_Should_Set_Active_To_False()
    {
        var product = new Product("Nome", 10, "Desc", 5, ProductCategory.Others);

        product.Disable();

        product.Active.Should().BeFalse();
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Enable_Should_Set_Active_To_True()
    {
        var product = new Product("Nome", 10, "Desc", 5, ProductCategory.Others);
        product.Disable();

        product.Enable();

        product.Active.Should().BeTrue();
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Change_Should_Update_Fields_And_UpdatedAt()
    {
        var product = new Product("Antigo", 10, "Velho", 5, ProductCategory.Others);

        product.Change("Novo", 15, "Atualizado", ProductCategory.Electronics);

        product.Name.Should().Be("Novo");
        product.Price.Should().Be(15);
        product.Description.Should().Be("Atualizado");
        product.Category.Should().Be(ProductCategory.Electronics);
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void DecreaseQuantity_Should_Decrease_Quantity_When_Valid()
    {
        var product = new Product("Produto", 100, "Descrição", 10, ProductCategory.Automotive);

        product.DecreaseQuantity(4);

        product.Quantity.Should().Be(6);
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void DecreaseQuantity_Should_Throw_When_Quantity_Is_Zero()
    {
        var product = new Product("Produto", 100, "Descrição", 10, ProductCategory.Automotive);

        Action act = () => product.DecreaseQuantity(0);

        act.Should().Throw<ArgumentOutOfRangeException>()
           .WithMessage("*Quantity must be greater than zero*");
    }

    [Fact]
    public void DecreaseQuantity_Should_Throw_When_Quantity_Is_Negative()
    {
        var product = new Product("Produto", 100, "Descrição", 10, ProductCategory.Automotive);

        Action act = () => product.DecreaseQuantity(-1);

        act.Should().Throw<ArgumentOutOfRangeException>()
           .WithMessage("*Quantity must be greater than zero*");
    }

    [Fact]
    public void DecreaseQuantity_Should_Throw_When_Insufficient_Stock()
    {
        var product = new Product("Produto", 100, "Descrição", 3, ProductCategory.Automotive);

        Action act = () => product.DecreaseQuantity(5);

        act.Should().Throw<DomainException>()
           .WithMessage("Insufficient stock for product*");
    }

    [Fact]
    public void DecreaseQuantity_Should_Update_UpdatedAt()
    {
        var product = new Product("Produto", 100, "Descrição", 5, ProductCategory.Automotive);
        var before = DateTime.UtcNow;

        product.DecreaseQuantity(1);

        product.UpdatedAt.Should().NotBeNull();
        product.UpdatedAt.Should().BeAfter(before);
    }
}
