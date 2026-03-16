using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // 🔥 ADD THIS
using ShopManagementAPI.Data;
using ShopManagementAPI.Models;

namespace ShopManagementAPI.Controllers;

[Authorize] // 🔐 ADD THIS
[ApiController]
[Route("api/items")]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ItemsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("shop/{shopId}")]
    public IActionResult GetByShop(int shopId)
    {
        var items = _context.Items.Where(i => i.ShopId == shopId).ToList();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var item = _context.Items.FirstOrDefault(i => i.ItemId == id);
        if (item == null)
            return NotFound("Item not found");

        return Ok(item);
    }

    [HttpPost]
    public IActionResult Create(Item item)
    {
        _context.Items.Add(item);
        _context.SaveChanges();
        return Ok(item);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Item updatedItem)
    {
        var item = _context.Items.FirstOrDefault(i => i.ItemId == id);
        if (item == null)
            return NotFound("Item not found");

        item.ItemName = updatedItem.ItemName;
        item.Quantity = updatedItem.Quantity;
        item.CostPrice = updatedItem.CostPrice;
        item.SellingPrice = updatedItem.SellingPrice;
        item.ExpiryDate = updatedItem.ExpiryDate;

        _context.SaveChanges();
        return Ok(item);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var item = _context.Items.FirstOrDefault(i => i.ItemId == id);
        if (item == null)
            return NotFound("Item not found");

        _context.Items.Remove(item);
        _context.SaveChanges();
        return Ok("Item deleted successfully");
    }

    [HttpGet("shop/{shopId}/expiring")]
    public IActionResult GetExpiring(int shopId, [FromQuery] int days = 30)
    {
        var threshold = DateTime.Now.AddDays(days);

        var items = _context.Items
            .Where(i => i.ShopId == shopId &&
                        i.ExpiryDate <= threshold &&
                        i.ExpiryDate >= DateTime.Now)
            .ToList();

        return Ok(items);
    }
}