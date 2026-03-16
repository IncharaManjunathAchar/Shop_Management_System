using Microsoft.AspNetCore.Mvc;
using ShopManagementAPI.Data;
using ShopManagementAPI.Models;

namespace ShopManagementAPI.Controllers;

[ApiController]
[Route("api/shops")]
public class ShopsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ShopsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_context.Shops.ToList());

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var shop = _context.Shops.Find(id);
        return shop == null ? NotFound() : Ok(shop);
    }

    [HttpPost]
    public IActionResult Create(Shop shop)
    {
        _context.Shops.Add(shop);
        _context.SaveChanges();
        return Ok(shop);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Shop updatedShop)
    {
        var shop = _context.Shops.Find(id);
        if (shop == null) return NotFound();

        shop.ShopName = updatedShop.ShopName;
        shop.ShopAddress = updatedShop.ShopAddress;
        shop.ContactNumber = updatedShop.ContactNumber;

        _context.SaveChanges();
        return Ok(shop);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var shop = _context.Shops.Find(id);
        if (shop == null) return NotFound();

        _context.Shops.Remove(shop);
        _context.SaveChanges();
        return Ok("Shop deleted successfully.");
    }
}
