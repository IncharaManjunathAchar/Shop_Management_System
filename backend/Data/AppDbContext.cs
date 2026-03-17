using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopManagementAPI.Models;

namespace ShopManagementAPI.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Shop> Shops { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Item>(e =>
            {
                e.Property(i => i.CostPrice).HasPrecision(18, 2);
                e.Property(i => i.SellingPrice).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Transaction>(e =>
            {
                e.Property(t => t.UnitPrice).HasPrecision(18, 2);
                e.Property(t => t.TotalAmount).HasPrecision(18, 2);
                e.HasOne(t => t.Shop)
                 .WithMany()
                 .HasForeignKey(t => t.ShopId)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<SubscriptionPlan>(e =>
            {
                e.Property(s => s.Price).HasPrecision(18, 2);
                e.HasData(
                    new SubscriptionPlan { PlanId = 1, PlanName = "Free Trial", DurationDays = 14, Price = 0, TrialDays = 14, MaxShops = 1, Description = "Try all features free for 14 days" },
                    new SubscriptionPlan { PlanId = 2, PlanName = "Monthly", DurationDays = 30, Price = 99, TrialDays = 0, MaxShops = 5, Description = "Full access, billed monthly" },
                    new SubscriptionPlan { PlanId = 3, PlanName = "Yearly", DurationDays = 365, Price = 999, TrialDays = 0, MaxShops = 7, Description = "Full access, billed yearly" }
                );
            });
        }
    }
}
