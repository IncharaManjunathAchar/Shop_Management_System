using Microsoft.EntityFrameworkCore;
using ShopManagementAPI.Models;

namespace ShopManagementAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Shop> Shops { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(e =>
        {
            e.Property(i => i.CostPrice).HasPrecision(18, 2);
            e.Property(i => i.SellingPrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<SubscriptionPlan>()
            .Property(s => s.Price).HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>(e =>
        {
            e.Property(t => t.UnitPrice).HasPrecision(18, 2);
            e.Property(t => t.TotalAmount).HasPrecision(18, 2);
        });
    }
}