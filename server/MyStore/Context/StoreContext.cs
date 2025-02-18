using Microsoft.EntityFrameworkCore;
using MyStore.Models;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MyStore.Data;

public class StoreContext : DbContext
{
    public StoreContext(DbContextOptions<StoreContext> options)
    : base(options)
    {
    }
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<User> Users => Set<User>();

    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion<string>(); // Storing enum as string

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>(); // Storing enum as string
    }
}