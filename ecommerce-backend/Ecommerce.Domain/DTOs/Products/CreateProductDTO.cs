using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.DTOs.Products;
        public class CreateProductDTO
        {
            [Required(ErrorMessage = "Le titre est obligatoire")]
            [StringLength(200, ErrorMessage = "Le titre ne doit pas dépasser 200 caractères")]
            public string Title { get; set; } = string.Empty;

            [StringLength(2000, ErrorMessage = "La description ne doit pas dépasser 2000 caractères")]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Le prix est obligatoire")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Le prix doit être supérieur à 0")]
            public decimal Price { get; set; }

            [Required(ErrorMessage = "Le stock est obligatoire")]
            [Range(0, int.MaxValue, ErrorMessage = "Le stock ne peut pas être négatif")]
            public int Stock { get; set; } = 0;

            [Required(ErrorMessage = "La catégorie est obligatoire")]
            public Guid CategoryId { get; set; }

            // 🏷️ Remise en pourcentage (ex: 10 = -10%)
            [Range(0, 100, ErrorMessage = "Le pourcentage de remise doit être entre 0 et 100")]
            public decimal? DiscountPercentage { get; set; }

            public DateTime? DiscountStartDate { get; set; }
            public DateTime? DiscountEndDate { get; set; }

            // 🚚 Prix de livraison
            [Range(0, double.MaxValue, ErrorMessage = "Le prix de livraison à domicile doit être positif")]
            public decimal DeliveryPriceMaison { get; set; } = 400;

            [Range(0, double.MaxValue, ErrorMessage = "Le prix de livraison au bureau doit être positif")]
            public decimal DeliveryPriceBureau { get; set; } = 200;

            // 🖼️ Image principale
            public IFormFile? Image { get; set; }

            // 📸 Galerie d’images
            public List<IFormFile>? Images { get; set; }

            // ⚙️ Variantes du produit (optionnel)
            public List<CreateProductVariantDTO>? Variants { get; set; }
        }

        // ✅ DTO pour variantes de produit
        public class CreateProductVariantDTO
        {
            [Required]
            public string Name { get; set; } = string.Empty; // Exemple: "Taille M" ou "Couleur Rouge"

            public decimal? AdditionalPrice { get; set; } // Ex: +500 DA pour une grande taille

            [Range(0, int.MaxValue, ErrorMessage = "Le stock ne peut pas être négatif")]
            public int VariantStock { get; set; } = 0;
        }
