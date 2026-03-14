using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManagementAPI.Data;
using ShopManagementAPI.Models;

namespace ShopManagementAPI.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    // -------------------- SHOPS --------------------

    // Get All Shops
    [HttpGet("shops")]
    public IActionResult GetShops()
    {
        var shops = _context.Shops.ToList();
        return Ok(shops);
    }

    // Get Shop By Id
    [HttpGet("shops/{id}")]
    public IActionResult GetShop(int id)
    {
        var shop = _context.Shops.Find(id);

        if (shop == null)
            return NotFound();

        return Ok(shop);
    }

    // Create Shop
    [HttpPost("shops")]
    public IActionResult CreateShop(Shop shop)
    {
        _context.Shops.Add(shop);
        _context.SaveChanges();

        return Ok(shop);
    }

    // Update Shop
    [HttpPut("shops/{id}")]
    public IActionResult UpdateShop(int id, Shop updatedShop)
    {
        var shop = _context.Shops.Find(id);

        if (shop == null)
            return NotFound();

        shop.ShopName = updatedShop.ShopName;
        shop.ShopAddress = updatedShop.ShopAddress;
        shop.ContactNumber = updatedShop.ContactNumber;

        _context.SaveChanges();

        return Ok(shop);
    }

    // Delete Shop
    [HttpDelete("shops/{id}")]
    public IActionResult DeleteShop(int id)
    {
        var shop = _context.Shops.Find(id);

        if (shop == null)
            return NotFound();

        _context.Shops.Remove(shop);
        _context.SaveChanges();

        return Ok("Shop deleted successfully");
    }

    // -------------------- SUBSCRIPTION PLANS --------------------

    // Get All Plans
    [HttpGet("subscriptionplans")]
    public IActionResult GetPlans()
    {
        var plans = _context.SubscriptionPlans.ToList();
        return Ok(plans);
    }

    // Create Plan
    [HttpPost("subscriptionplans")]
    public IActionResult CreatePlan(SubscriptionPlan plan)
    {
        _context.SubscriptionPlans.Add(plan);
        _context.SaveChanges();

        return Ok(plan);
    }

    // Update Plan
    [HttpPut("subscriptionplans/{id}")]
public IActionResult UpdatePlan(int id, SubscriptionPlan updatedPlan)
{
    var plan = _context.SubscriptionPlans.Find(id);

    if (plan == null)
        return NotFound();

    plan.PlanName = updatedPlan.PlanName;
    plan.DurationDays = updatedPlan.DurationDays;
    plan.Price = updatedPlan.Price;
    plan.TrialDays = updatedPlan.TrialDays;
    plan.Description = updatedPlan.Description;

    _context.SaveChanges();

    return Ok(plan);
}

    // Delete Plan
    [HttpDelete("subscriptionplans/{id}")]
    public IActionResult DeletePlan(int id)
    {
        var plan = _context.SubscriptionPlans.Find(id);

        if (plan == null)
            return NotFound();

        _context.SubscriptionPlans.Remove(plan);
        _context.SaveChanges();

        return Ok("Plan deleted successfully");
    }

    // -------------------- TRANSACTIONS --------------------

    // Get All Transactions
    [HttpGet("transactions")]
    public IActionResult GetTransactions()
    {
        var transactions = _context.Transactions.ToList();
        return Ok(transactions);
    }

    // Get Transaction By Id
    [HttpGet("transactions/{id}")]
    public IActionResult GetTransaction(int id)
    {
        var transaction = _context.Transactions.Find(id);

        if (transaction == null)
            return NotFound();

        return Ok(transaction);
    }

    // -------------------- USER SUBSCRIPTIONS --------------------

    // Get All User Subscriptions
   [HttpGet("usersubscriptions")]
public IActionResult GetUserSubscriptions()
{
    var subscriptions = _context.UserSubscriptions.ToList();
    return Ok(subscriptions);
}

    // Get User Subscription By Id
    [HttpGet("usersubscriptions/{id}")]
public IActionResult GetUserSubscription(int id)
{
    var subscription = _context.UserSubscriptions
        .FirstOrDefault(x => x.SubscriptionId == id);

    if (subscription == null)
        return NotFound();

    return Ok(subscription);
}
}