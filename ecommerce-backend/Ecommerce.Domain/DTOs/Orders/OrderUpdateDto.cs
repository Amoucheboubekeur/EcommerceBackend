namespace Ecommerce.Domain.DTOs.Orders;

    public class OrderUpdateDto
    {
        public string? ShippingAddress { get; set; }
        public List<OrderItemUpdateDto>? Items { get; set; }
    }

    public class OrderItemUpdateDto
    {

        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }


