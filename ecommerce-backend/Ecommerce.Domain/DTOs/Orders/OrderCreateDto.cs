namespace Ecommerce.Domain.DTOs.Orders;
    public class OrderCreateDto
    {
        public string? ShippingAddress { get; set; }
        public string Wilaya { get; set; }
        public string Commune { get; set; }
        public string DeliveryMethod { get; set; }
        public decimal DeliveryPrice { get; set; }
        public List<OrderItemCreateDto> Items { get; set; } = [];
        public decimal? Total { get; set; }
    }

    public class OrderItemCreateDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
       public Guid? VariantId { get; set; } 

    }

    public class UpdateOrderStatusDto
    {
        public string Status { get; set; } = default!;
    }

