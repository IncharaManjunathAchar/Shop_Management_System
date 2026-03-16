using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // 🔥 ADD THIS
using ShopManagementAPI.Data;
using ShopManagementAPI.Models;

namespace ShopManagementAPI.Controllers;

[Authorize] // 🔐 ADD THIS
[ApiController]
[Route("api/subscriptions")]
public class SubscriptionController : ControllerBase
{
    private readonly AppDbContext _context;

    public SubscriptionController(AppDbContext context)
    {
        _context = context;
    }

    // ---- Subscription Plans ----

    [HttpGet("plans")]
    public IActionResult GetPlans()
    {
        var plans = _context.SubscriptionPlans.ToList();
        return Ok(plans);
    }

    [HttpPost("plans")]
    public IActionResult CreatePlan(SubscriptionPlan plan)
    {
        _context.SubscriptionPlans.Add(plan);
        _context.SaveChanges();
        return Ok(plan);
    }

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

        _context.SaveChanges();
        return Ok(plan);
    }

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

        return Ok(subscription);
    }

    [HttpGet("users/{userId}/active")]
    public IActionResult GetActiveSubscription(string userId)
    {
        var subscription = _context.UserSubscriptions
            .Where(s => s.UserId == userId && s.ExpiryDate >= DateTime.Now)
            .OrderByDescending(s => s.ExpiryDate)
            .FirstOrDefault();

        if (subscription == null)
            return NotFound("No active subscription found");

        return Ok(subscription);
    }

    [HttpPost("users/{userId}/subscribe/{planId}")]
    public IActionResult Subscribe(string userId, int planId)
    {
        var plan = _context.SubscriptionPlans.Find(planId);
        if (plan == null)
            return NotFound("Plan not found");

        var hasActive = _context.UserSubscriptions
            .Any(s => s.UserId == userId && s.ExpiryDate >= DateTime.Now);

        if (hasActive)
            return BadRequest("User already has an active subscription");

        var subscription = new UserSubscription
        {
            UserId = userId,
            PlanId = planId,
            StartDate = DateTime.Now,
            ExpiryDate = DateTime.Now.AddDays(plan.DurationDays),
            SubscriptionType = plan.TrialDays > 0 ? "Trial" : "Paid",
            PaymentStatus = plan.Price == 0 ? "Free" : "Paid"
        };

        _context.UserSubscriptions.Add(subscription);
        _context.SaveChanges();

        return Ok(subscription);
    }

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