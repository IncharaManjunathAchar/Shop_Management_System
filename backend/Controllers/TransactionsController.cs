using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // 🔥 ADD THIS
using ShopManagementAPI.Data;
using ShopManagementAPI.Models;

namespace ShopManagementAPI.Controllers;

[Authorize] // 🔐 ADD THIS
[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransactionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var transactions = _context.Transactions.ToList();
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var transaction = _context.Transactions.Find(id);
        if (transaction == null)
            return NotFound("Transaction not found");

        return Ok(transaction);
    }

    [HttpGet("shop/{shopId}")]
    public IActionResult GetByShop(int shopId)
    {
        var transactions = _context.Transactions
            .Where(t => t.ShopId == shopId)
            .ToList();

        return Ok(transactions);
    }

    [HttpPost]
    public IActionResult Create(Transaction transaction)
    {
        var item = _context.Items
            .FirstOrDefault(i => i.ItemId == transaction.ItemId && i.ShopId == transaction.ShopId);

        if (item == null)
            return NotFound("Item not found in this shop");

        if (transaction.TransactionType == "Sale")
        {
            if (item.Quantity < transaction.Quantity)
                return BadRequest("Insufficient stock");

            item.Quantity -= transaction.Quantity;
        }
        else if (transaction.TransactionType == "Purchase")
        {
            item.Quantity += transaction.Quantity;
        }
        else
        {
            return BadRequest("TransactionType must be 'Sale' or 'Purchase'");
        }

        transaction.TotalAmount = transaction.Quantity * transaction.UnitPrice;
        transaction.TransactionDate = DateTime.Now;

        _context.Transactions.Add(transaction);
        _context.SaveChanges();

        return Ok(transaction);
    }
}