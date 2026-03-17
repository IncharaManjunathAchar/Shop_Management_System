using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ShopManagementAPI.Models;

public class UserSubscription
{
    [Key]
    public int SubscriptionId { get; set; }

    public required string UserId { get; set; }
    [ForeignKey("UserId")]
    public IdentityUser? User { get; set; }

    public int PlanId { get; set; }
    [ForeignKey("PlanId")]
    public SubscriptionPlan? Plan { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public required string SubscriptionType { get; set; }

    public required string PaymentStatus { get; set; }
}