using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ShopManagementAPI.Data;
using ShopManagementAPI.Models;
using ShopManagementAPI.Services;
using System.Security.Claims;

namespace ShopManagementAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/subscriptions")]
public class SubscriptionController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly EmailService _emailService;

    public SubscriptionController(AppDbContext context, UserManager<IdentityUser> userManager, EmailService emailService)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
    }

    // ---- Subscription Plans ----

    [HttpGet("plans")]
    public IActionResult GetPlans()
    {
        var plans = _context.SubscriptionPlans.ToList();
        return Ok(plans);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("plans")]
    public IActionResult CreatePlan(SubscriptionPlan plan)
    {
        _context.SubscriptionPlans.Add(plan);
        _context.SaveChanges();
        return Ok(plan);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("plans/{id}")]
    public IActionResult UpdatePlan(int id, SubscriptionPlan updatedPlan)
    {
        var plan = _context.SubscriptionPlans.Find(id);
        if (plan == null)
            return NotFound("Plan not found");

        plan.PlanName = updatedPlan.PlanName;
        plan.DurationDays = updatedPlan.DurationDays;
        plan.Price = updatedPlan.Price;
        plan.TrialDays = updatedPlan.TrialDays;
        plan.Description = updatedPlan.Description;
        plan.MaxShops = updatedPlan.MaxShops;

        _context.SaveChanges();
        return Ok(plan);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("plans/{id}")]
    public IActionResult DeletePlan(int id)
    {
        var plan = _context.SubscriptionPlans.Find(id);
        if (plan == null)
            return NotFound("Plan not found");

        _context.SubscriptionPlans.Remove(plan);
        _context.SaveChanges();
        return Ok("Plan deleted successfully");
    }

    // ---- User Subscriptions ----

    [Authorize(Roles = "Admin")]
    [HttpGet("users")]
    public IActionResult GetAllUserSubscriptions()
    {
        var users = _context.UserSubscriptions.ToList();
        return Ok(users);
    }

    [HttpGet("users/{id}")]
    public IActionResult GetUserSubscription(int id)
    {
        var subscription = _context.UserSubscriptions.Find(id);
        if (subscription == null)
            return NotFound("Subscription not found");

        var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin") && subscription.UserId != userId)
            return Forbid();

        return Ok(subscription);
    }

    [HttpGet("users/{userId}/active")]
    public IActionResult GetActiveSubscription(string userId)
    {
        var requestingUserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin") && userId != requestingUserId)
            return Forbid();

        var subscription = _context.UserSubscriptions
            .Where(s => s.UserId == userId && s.ExpiryDate >= DateTime.Now)
            .OrderByDescending(s => s.ExpiryDate)
            .FirstOrDefault();

        if (subscription == null)
            return NotFound("No active subscription found");

        return Ok(subscription);
    }

    [HttpPost("users/{userId}/subscribe/{planId}")]
    public async Task<IActionResult> Subscribe(string userId, int planId)
    {
        var requestingUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin") && userId != requestingUserId)
            return Forbid();

        var plan = _context.SubscriptionPlans.Find(planId);
        if (plan == null)
            return NotFound("Plan not found");

        var hasActive = _context.UserSubscriptions
            .Any(s => s.UserId == userId && s.Status == "Approved" && s.ExpiryDate >= DateTime.Now);

        if (hasActive)
            return BadRequest("User already has an active subscription");

        var hasPending = _context.UserSubscriptions
            .Any(s => s.UserId == userId && s.Status == "Pending");

        if (hasPending)
            return BadRequest("User already has a pending subscription request");

        var subscription = new UserSubscription
        {
            UserId = userId,
            PlanId = planId,
            StartDate = DateTime.Now,
            ExpiryDate = DateTime.Now.AddDays(plan.DurationDays),
            SubscriptionType = plan.TrialDays > 0 ? "Trial" : "Paid",
            PaymentStatus = plan.Price == 0 ? "Free" : "Paid",
            Status = "Pending"
        };

        _context.UserSubscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Subscription request submitted. Awaiting admin approval.", subscription });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("pending")]
    public IActionResult GetPendingSubscriptions()
    {
        var pending = _context.UserSubscriptions
            .Where(s => s.Status == "Pending")
            .Select(s => new
            {
                s.SubscriptionId,
                s.UserId,
                Username = s.User != null ? s.User.UserName : null,
                s.PlanId,
                PlanName = s.Plan != null ? s.Plan.PlanName : null,
                s.StartDate,
                s.ExpiryDate,
                s.SubscriptionType,
                s.PaymentStatus,
                s.Status
            })
            .ToList();

        return Ok(pending);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveSubscription(int id)
    {
        var subscription = _context.UserSubscriptions.Find(id);
        if (subscription == null)
            return NotFound("Subscription not found");

        if (subscription.Status != "Pending")
            return BadRequest($"Subscription is already {subscription.Status}");

        subscription.Status = "Approved";
        subscription.StartDate = DateTime.Now;
        subscription.ExpiryDate = DateTime.Now.AddDays(
            (_context.SubscriptionPlans.Find(subscription.PlanId))?.DurationDays ?? 30);

        await _context.SaveChangesAsync();

        var user = await _userManager.FindByIdAsync(subscription.UserId);
        if (user?.Email != null)
        {
            try
            {
                var plan = _context.SubscriptionPlans.Find(subscription.PlanId);
                await _emailService.SendAsync(
                    user.Email, user.UserName!,
                    "Subscription Approved",
                    $"Dear {user.UserName},\n\nYour subscription to '{plan?.PlanName}' has been approved.\n" +
                    $"Expiry Date: {subscription.ExpiryDate:dd MMM yyyy}\n\nShop Management System"
                );
            }
            catch { }
        }

        return Ok(new { message = "Subscription approved.", subscription });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectSubscription(int id)
    {
        var subscription = _context.UserSubscriptions.Find(id);
        if (subscription == null)
            return NotFound("Subscription not found");

        if (subscription.Status != "Pending")
            return BadRequest($"Subscription is already {subscription.Status}");

        subscription.Status = "Rejected";
        await _context.SaveChangesAsync();

        var user = await _userManager.FindByIdAsync(subscription.UserId);
        if (user?.Email != null)
        {
            try
            {
                await _emailService.SendAsync(
                    user.Email, user.UserName!,
                    "Subscription Rejected",
                    $"Dear {user.UserName},\n\nYour subscription request has been rejected.\nPlease contact support.\n\nShop Management System"
                );
            }
            catch { }
        }

        return Ok(new { message = "Subscription rejected.", subscription });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("users/{id}")]
    public IActionResult DeleteUserSubscription(int id)
    {
        var subscription = _context.UserSubscriptions.Find(id);
        if (subscription == null)
            return NotFound("Subscription not found");

        _context.UserSubscriptions.Remove(subscription);
        _context.SaveChanges();

        return Ok("Subscription deleted successfully");
    }
}