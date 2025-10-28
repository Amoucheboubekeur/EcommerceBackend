namespace Ecommerce.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("Users")]

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // Adresse simplifiée
    public string Address { get; set; } = string.Empty;
    public string Commune { get; set; } = string.Empty;
    public string Wilaya { get; set; } = string.Empty;
    public string? PostalCode { get; set; } = string.Empty;
    public bool IsGuest { get; set; } = false;
    // Navigation
    [JsonIgnore]
    public List<Order> Orders { get; set; } = [];
}


