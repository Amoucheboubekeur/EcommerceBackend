using Ecommerce.Domain.DTOs.Products;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ecommerce.Domain.Entities;

[Table("Products")]
public class Product(string title, string? description, decimal price, int stock, Guid categoryId, string userId, decimal? discountPercentage, DateTime? discountStartDate, DateTime? discountEndDate, decimal deliveryPriceMaison, decimal deliveryPriceBureau)
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    [Required, MaxLength(150)]
    public string Title { get; private set; } = title;

    public string? Description { get; private set; } = description;

    [Range(0, double.MaxValue)]
    public decimal Price { get; private set; } = price;

    public string? ImageUrl { get; private set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; private set; } = stock;

    public void UpdateStock(int Quantity)
    {
        if (Quantity > 0 && Stock >= Quantity)
            Stock -= Quantity;
    }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public decimal? DiscountPercentage { get; private set; } = discountPercentage;
    public DateTime? DiscountStartDate { get; private set; } = discountStartDate;
    public DateTime? DiscountEndDate { get; private set; } = discountEndDate;

    public decimal DeliveryPriceMaison { get; private set; } = deliveryPriceMaison;
    public decimal DeliveryPriceBureau { get; private set; } = deliveryPriceBureau;


    // ✅ CORRECTION : Collections avec backing fields mais accessibles
    private List<ProductImage> _gallery = new();
    public IReadOnlyCollection<ProductImage> Gallery => _gallery;

    private List<ProductVariant> _variants = new();
   
    public IReadOnlyCollection<ProductVariant> Variants => _variants;

    // ✅ CORRECTION : Méthode Update complète
    public void Update(string title, string description, decimal price, string? imageUrl, int stock,
        decimal? discountPercentage, DateTime? discountStartDate, DateTime? discountEndDate,
        decimal deliveryPriceMaison, decimal deliveryPriceBureau, Guid categoryId)
    {
        this.Title = title;
        this.Description = description;
        this.Price = price;
        this.Stock = stock;
        this.ImageUrl = imageUrl;
        this.DiscountPercentage = discountPercentage;
        this.DiscountStartDate = discountStartDate;
        this.DiscountEndDate = discountEndDate;
        this.DeliveryPriceBureau = deliveryPriceBureau;
        this.DeliveryPriceMaison = deliveryPriceMaison;
        this.CategoryId = categoryId; // ✅ AJOUT IMPORTANT
    }

    // ✅ CORRECTION : ReplaceImages avec Clear + Add
    public void ReplaceImages(IEnumerable<string> newUrls)
    {
       // _gallery.Clear(); // ✅ VIDE la collection d'abord
        foreach (var url in newUrls)
        {
            _gallery.Add(new ProductImage(url, false, this.Id));
        }
    }

    // ✅ CORRECTION : ReplaceVariants avec Clear + Add
    public void ReplaceVariants(IEnumerable<CreateProductVariantDTO> newVariants)
    {
       _variants.Clear(); // ✅ VIDE la collection d'abord
        foreach (var v in newVariants)
        {
            _variants.Add(new ProductVariant(v.Name, "", v.AdditionalPrice, v.VariantStock, this.Id));
        }
    }


    public void AddImages(ProductImage newImages)
    {
        _gallery.Add(newImages);
    }

    public void AddVariants(ProductVariant newVariants)
    {
        _variants.Add(newVariants);
    }
    // ✅ Relations
    public Guid CategoryId { get; private set; } = categoryId;
    public Category Category { get; private set; } = null!;

    public List<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

    public int? SalesCount { get; private set; } = 0;
    public string? UserId { get; private set; } = userId;
    public ApplicationUser? User { get; private set; }

    [NotMapped]
    public decimal FinalPrice
    {
        get
        {
            if (DiscountPercentage.HasValue && IsDiscountActive())
            {
                var discountAmount = (Price * DiscountPercentage.Value) / 100;
                return Math.Round(Price - discountAmount, 2);
            }
            return Price;
        }
    }

    private bool IsDiscountActive()
    {
        if (!DiscountStartDate.HasValue || !DiscountEndDate.HasValue)
            return false;

        var now = DateTime.UtcNow;
        return now >= DiscountStartDate.Value && now <= DiscountEndDate.Value;
    }
}
[Table("ProductVariants", Schema = "catalog")]
public class ProductVariant
{
    private ProductVariant() { }
    public ProductVariant(string name, string type, decimal? additionalPrice, int variantStock, Guid productId) { 
        this.Name = name;
        this.Type = type;
        this.AdditionalPrice = additionalPrice;
        this.VariantStock = variantStock;
        this.ProductId = productId;
    }

    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    [Required, MaxLength(100)]
    public string Name { get; private set; }

    [MaxLength(50)]
    public string? Type { get; private set; }

    public decimal? AdditionalPrice { get; private set; }

    public int VariantStock { get; private set; }

    public void UpdateStock(int Quantity)
    {
        if(Quantity > 0 && VariantStock>= Quantity) 
        VariantStock -= Quantity;
    }
    public Guid ProductId { get; private set; }

    [JsonIgnore]
    public Product Product { get; private set; } = null!;
}


[Table("ProductImages", Schema = "catalog")]
public class ProductImage
{
    private ProductImage(){ }
    public ProductImage(string url, bool isMain, Guid productId)
    {
        Url = url;
        IsMain = isMain;
        ProductId = productId;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Url { get; private set; } 
    public bool IsMain { get; private set; }

    public Guid ProductId { get; private set; }

    [JsonIgnore]
    public Product Product { get; private set; } = null!;
}
