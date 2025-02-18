using MyStore.Models;

namespace MyStore.Data
{
    public static class DbInitializer
    {
        public static void Initialize(StoreContext context)
        {

            if (context.Categories.Any()
                && context.Products.Any())
            {
                return;   // DB has been seeded
            }

            var categories = new Category[]
            {
                new Category { Name = "Electronics" },
                new Category { Name = "Books" },
                new Category { Name = "Clothing" }
            };

            var products = new Product[]
            {
                new Product {
                    Name = "Smartphone",
                    Description = "A new smartphone",
                    Price = 699.99M,
                    StockQuantity = 50,
                    ImageUrl = null,
                    CategoryId = 1
                },
                new Product {
                    Name = "Novel",
                    Description = "An interesting book",
                    Price = 19.99M,
                    StockQuantity = 100,
                    ImageUrl = null,
                    CategoryId = 2
                }
            };

            context.Categories.AddRange(categories);
            context.Products.AddRange(products);

            context.SaveChanges();
        }
    }
}