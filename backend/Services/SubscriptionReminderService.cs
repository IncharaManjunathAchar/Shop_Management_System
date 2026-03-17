using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopManagementAPI.Data;

namespace ShopManagementAPI.Services;

public class SubscriptionReminderService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<SubscriptionReminderService> _logger;

    public SubscriptionReminderService(IServiceProvider services, ILogger<SubscriptionReminderService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAndSendReminders();
            // Wait 24 hours before checking again
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task CheckAndSendReminders()
    {
        using var scope = _services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

        var now = DateTime.Now;
        var reminderWindow = now.AddDays(2);

        var expiringSubscriptions = await context.UserSubscriptions
            .Where(s => s.ExpiryDate >= now && s.ExpiryDate <= reminderWindow)
            .ToListAsync();

        foreach (var subscription in expiringSubscriptions)
        {
            var user = await userManager.FindByIdAsync(subscription.UserId);
            if (user?.Email == null) continue;

            var daysLeft = (subscription.ExpiryDate - now).Days;
            var message = $"Dear {user.UserName},\n\nYour subscription will expire on {subscription.ExpiryDate:dd MMM yyyy}. " +
                          $"You have {daysLeft} day(s) left.\n\nPlease renew your subscription to continue using the system.\n\nShop Management System";

            try
            {
                await emailService.SendAsync(user.Email, user.UserName!, "Subscription Expiry Reminder", message);
                _logger.LogInformation("Reminder sent to {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send reminder to {Email}", user.Email);
            }
        }
    }
}
