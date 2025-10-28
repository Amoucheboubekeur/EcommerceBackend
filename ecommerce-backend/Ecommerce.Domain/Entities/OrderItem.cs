
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ecommerce.Domain.Entities;

[Table("OrderItems")]
public class OrderItem
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }
    [JsonIgnore]
    public Order Order { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public string NameVariant { get;  set; }

  
    public decimal? AdditionalPriceVariant { get;set; }
}