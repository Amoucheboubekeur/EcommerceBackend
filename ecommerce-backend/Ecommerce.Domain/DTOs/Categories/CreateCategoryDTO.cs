
using System.ComponentModel.DataAnnotations;

  namespace Ecommerce.Domain.DTOs.Categories
    {
        public class CreateCategoryDTO
        {
            [Required(ErrorMessage = "Le nom est obligatoire")]
            [StringLength(200, ErrorMessage = "Le nom ne doit pas dépasser 200 caractères")]
            public string Name { get; set; } = string.Empty;

        }
    }


