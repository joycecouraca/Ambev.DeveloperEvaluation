using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.ORM;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.Integration.Fixture;

public static class SeedDataExtensions
{
    public static Guid SeededUserId { get; private set; }

    public static void EnsureSeeded(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();

        Seed(context);
    }

    public static void Seed(DefaultContext context)
    {
        if (!context.Users.Any())
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "Cliente Teste",
                Email = "cliente@teste.com",
                Phone = "11999999999",
                Password = "Senha@123", // em produção, deveria ser hash
                Role = UserRole.Customer,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            SeededUserId = user.Id; // <-- salva o ID para uso no teste

            context.Users.Add(user);
        }

        if (!context.Products.Any())
        {
            var products = new List<Product>
            {
                new Product(
                    name: "Cerveja IPA",
                    price: 9.90m,
                    description: "Uma cerveja artesanal IPA",
                    quantity: 100,
                    category: ProductCategory.Others
                ),
                new Product(
                    name: "Smartphone Galaxy",
                    price: 1999.99m,
                    description: "Celular Samsung com Android",
                    quantity: 15,
                    category: ProductCategory.Electronics
                ),
                new Product(
                    name: "Camisa Polo",
                    price: 89.90m,
                    description: "Camisa polo masculina, tamanho M",
                    quantity: 25,
                    category: ProductCategory.Clothing
                ),
                new Product(
                    name: "Geladeira Frost Free",
                    price: 2399.00m,
                    description: "Geladeira 2 portas com tecnologia Frost Free",
                    quantity: 5,
                    category: ProductCategory.HomeAppliances
                ),
                new Product(
                    name: "Livro .NET para Profissionais",
                    price: 129.90m,
                    description: "Livro técnico sobre desenvolvimento com .NET",
                    quantity: 12,
                    category: ProductCategory.Books
                )
            };

            context.Products.AddRange(products);
        }

        context.SaveChanges();

        if (!context.Sales.Any())
        {
            // Pega o primeiro usuário e produto disponíveis (que foram criados pelo seed)
            var user = context.Users.FirstOrDefault();
            var product = context.Products.FirstOrDefault();

            if (user != null && product != null)
            {
                var sale = Sale.Create(user, user, DateTime.UtcNow, "Unidade Seed");

                sale.AddItems(new SaleItem(product, 1)); // Adiciona item à venda

                context.Sales.Add(sale);
                context.SaveChanges();
            }
        }

    }
}