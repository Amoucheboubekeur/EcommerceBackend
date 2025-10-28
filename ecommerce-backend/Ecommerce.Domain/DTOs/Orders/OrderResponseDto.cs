namespace Ecommerce.Domain.DTOs.Orders;

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public decimal Total { get; set; }
    public string? ShippingAddress { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }

    public List<OrderItemResponseDto> Items { get; set; } = new();
}

public class OrderItemResponseDto
{
    public Guid ProductId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
