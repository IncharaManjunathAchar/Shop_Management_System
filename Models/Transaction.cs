namespace ShopManagementAPI.Models;

public class Transaction
{
    public int TransactionId { get; set; }

    public int ShopId { get; set; }

    public int ItemId { get; set; }

    public string TransactionType { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime TransactionDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}