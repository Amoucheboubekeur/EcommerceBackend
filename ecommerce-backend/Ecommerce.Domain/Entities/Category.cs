
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ecommerce.Domain.Entities;
[Table("Categories")]

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    [JsonIgnore] // 🚀 Empêche la boucle Category -> Products -> Category
    public List<Product> Products { get; set; } = new List<Product>();
}


