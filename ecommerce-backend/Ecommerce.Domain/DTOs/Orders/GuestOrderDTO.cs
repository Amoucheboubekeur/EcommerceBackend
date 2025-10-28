namespace Ecommerce.Domain.DTOs.Orders;

public class GuestOrderDTO
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Wilaya { get; set; } = "";
    public string Commune { get; set; } = "";
    public string DeliveryMethod { get; set; } = ""; // "maison" ou "bureau"
    public decimal DeliveryPrice { get; set; }
    public string? VariantId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }

   
}

