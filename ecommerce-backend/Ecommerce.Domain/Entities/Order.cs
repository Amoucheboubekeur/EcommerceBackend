
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ecommerce.Domain.Entities;

[Table("Orders")]

public class Order
{
    public Guid Id { get; set; }

    // FK vers IdentityUser (string par défaut)
    public string UserId { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;

    public string Status { get; set; } = "Pending"; // Pending | Confirmed | Canceled
    public void UpdateStatus(string newStatus)
    {
        var allowed = new[] { "Pending", "Confirmed", "Canceled" };
        if (!allowed.Contains(newStatus))
            throw new InvalidOperationException($"Le statut '{newStatus}' n'est pas valide.");

        Status = newStatus;
    }
    public string? ShippingAddress { get; set; }

    public DeliveryMethod DeliveryMethod { get; set; } = DeliveryMethod.Maison;

    public string? Wilaya { get; set; }

    public string? Commune { get; set; }
    public decimal DeliveryPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }

    public decimal? Total { get; set; }

    // Navigation
    [JsonIgnore]
    public List<OrderItem> Items { get; set; } = [];
}

public enum DeliveryMethod
{
    Maison = 1,
    BureauLivraison = 2
}


