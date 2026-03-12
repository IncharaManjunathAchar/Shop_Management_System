using System.ComponentModel.DataAnnotations;

namespace ShopManagementAPI.Models;

public class UserSubscription
{
    [Key]
    public int SubscriptionId { get; set; }

    public required string UserId { get; set; }

    public int PlanId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public required string SubscriptionType { get; set; }

    public required string PaymentStatus { get; set; }
}