using ShopManagementAPI.Data;
using System.Security.Claims;

namespace ShopManagementAPI.Middleware;

public class SubscriptionMiddleware
{
    private readonly RequestDelegate _next;

    public SubscriptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        var isWhitelisted =
            path.Contains("/api/auth") ||
            path.Contains("/api/subscriptions") ||
            path.Contains("/swagger");

        if (!isWhitelisted && context.User.Identity?.IsAuthenticated == true && context.User.IsInRole("Shopkeeper"))
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var hasActive = db.UserSubscriptions
                .Any(s => s.UserId == userId && s.Status == "Approved" && s.ExpiryDate >= DateTime.Now);

            if (!hasActive)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { message = "Your subscription has expired. Please renew to continue." });
                return;
            }
        }

        await _next(context);
    }
}
