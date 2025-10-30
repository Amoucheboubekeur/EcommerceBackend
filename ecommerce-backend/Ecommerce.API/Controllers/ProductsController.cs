using Ecommerce.Application.Common.Pagination;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.DTOs.Products;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Controllers;
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,Admin")] // Seul le SuperAdmin peut accéder ici


    public class ProductsController(IProductService service) : ControllerBase
    {
        private readonly IProductService _service = service;

    //ok
    [HttpGet("public")]
    [AllowAnonymous] // 🔓 Accessible sans authentification
    public async Task<IActionResult> GetPublicProducts([FromQuery] PaginationParams pagination, [FromQuery] Guid? categoryId = null)
    {
        var result = await _service.GetPublicPagedProductsAsync(pagination, categoryId);
        return Ok(result);
    }

    [HttpGet]//ok
    public async Task<IActionResult> GetAll()
    {
        var products = await _service.GetAllAsync();
        return Ok(products);
    }

    // ✅ Meilleures ventes (Best Sellers)
    [AllowAnonymous]
    [HttpGet("best-sellers")]
    public async Task<IActionResult> GetBestSellers()
    {
        var products = await _service.GetBestSellersAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    [AllowAnonymous] // 🔓 Accessible sans authentification

    public async Task<IActionResult> GetById(string id)
    {
   
        // 🆔 Conversion sécurisée de string vers Guid
        if (!Guid.TryParse(id, out var productId))
        {
            return BadRequest(new { message = "Identifiant du produit invalide." });
        }

        // 📦 Récupérer le produit via le service
        var product = await _service.GetByIdAsync(productId);

        if (product == null)
        {
            return NotFound(new { message = "Produit introuvable." });
        }

        return Ok(product);
    }



    [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateProductDTO dto)
        {
            var galleryUrls = new List<string>();
            string? mainImageUrl = null;

            // 📌 Sauvegarder l’image principale
            if (dto.Image != null && dto.Image.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads/products", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync(stream);

                mainImageUrl = $"/uploads/products/{fileName}";
            }

            // 📌 Sauvegarder la galerie
            if (dto.Images != null && dto.Images.Count > 0)
            {
                foreach (var img in dto.Images)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(img.FileName)}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads/products", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await img.CopyToAsync(stream);

                    galleryUrls.Add($"/uploads/products/{fileName}");
                }
            }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized(new { message = "Vous devez être connecté pour créer un produit." });
        }
        // 📌 Appeler le service
        var product = await _service.CreateAsync(dto, mainImageUrl, galleryUrls, userId);

            return Ok(new
            {
                message = "Produit ajouté avec succès ✅",
                product
            });
        }


        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(Guid id, [FromForm] CreateProductDTO dto)
        {
            var galleryUrls = new List<string>();
            string? mainImageUrl = null;

            // 📌 Sauvegarder l’image principale (comme dans Create)
            if (dto.Image != null && dto.Image.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads/products", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync(stream);

                mainImageUrl = $"/uploads/products/{fileName}";
            }

            // 📌 Sauvegarder la galerie (comme dans Create)
            if (dto.Images != null && dto.Images.Count > 0)
            {
                foreach (var img in dto.Images)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(img.FileName)}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads/products", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await img.CopyToAsync(stream);

                    galleryUrls.Add($"/uploads/products/{fileName}");
                }
            }

        // 🔐 Vérifie si l'utilisateur est connecté
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized(new { message = "Vous devez être connecté pour consulter ce produit." });
        }
        // 📌 Appeler le service avec les images comme Create
        var updatedProduct = await _service.UpdateAsync(id, dto, mainImageUrl, galleryUrls);

            return Ok(new
            {
                message = "Produit mis à jour avec succès ✅",
                updatedProduct
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpDelete("{productId}/images/{imageId}")]
        public async Task<IActionResult> DeleteImage(Guid productId, Guid imageId)
        {
            var result = await _service.DeleteImageAsync(productId, imageId);

            if (!result)
                return NotFound(new { message = "Produit ou image introuvable" });

            return NoContent();
        }

        [HttpGet("GetPaged")]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams pagination)
        {
        // 🔐 Vérifie si l'utilisateur est connecté
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized(new { message = "Vous devez être connecté pour consulter ce produit." });
        }

           var result = await _service.GetPagedProductsAsync(pagination, userId);
            return Ok(result);
        }

}

