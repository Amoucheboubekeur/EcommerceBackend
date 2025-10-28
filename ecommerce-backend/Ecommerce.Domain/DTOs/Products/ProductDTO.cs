namespace Ecommerce.Domain.DTOs.Products;
public class ProductDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int QuantitySold { get; set; }
    public int? SalesCount { get; set; }

}
