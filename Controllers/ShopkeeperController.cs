using Microsoft.AspNetCore.Mvc;
using ShopManagementAPI.Data;

namespace ShopManagementAPI.Controllers;

[ApiController]
[Route("api/shop")]
public class ShopController : ControllerBase
{
    private readonly AppDbContext _context;

    public ShopController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{shopId}/items")]
    public IActionResult GetItems(int shopId)
    {
        var items = _context.Items.Where(i => i.ShopId == shopId).ToList();
        return Ok(items);
    }
}