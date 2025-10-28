using Ecommerce.Application.Common.Pagination;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.DTOs.Products;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces;


namespace Ecommerce.Application.Services
{
    public class ProductService(IProductRepository productRepo, IUserRepository userRepo, IVariantProductRepository variantRepo) : IProductService
    {
        private readonly IProductRepository _productRepo = productRepo;
        private readonly IUserRepository _userRepo = userRepo;
        private readonly IVariantProductRepository _variantRepo = variantRepo;



        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _productRepo.GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _productRepo.GetByIdAsync(id);
        }

        public async Task<Product> CreateAsync(CreateProductDTO dto, string? mainImageUrl, List<string>? galleryUrls, string userId)
        {
           
            // 1. Créer l'entité Product
            var product = new Product(
                title: dto.Title,
                description: dto.Description,
                price: dto.Price,
                stock: dto.Stock,
                categoryId: dto.CategoryId,
                userId: userId,
                discountPercentage: dto.DiscountPercentage,
                discountStartDate: DateTime.SpecifyKind(dto.DiscountStartDate ?? DateTime.UtcNow, DateTimeKind.Utc),
                discountEndDate: DateTime.SpecifyKind(dto.DiscountEndDate ?? DateTime.UtcNow, DateTimeKind.Utc),
                deliveryPriceMaison: dto.DeliveryPriceMaison,
                deliveryPriceBureau: dto.DeliveryPriceBureau

            );

            // 2. Ajouter l'image principale
            if (!string.IsNullOrEmpty(mainImageUrl))
            {
                product.AddImages(new ProductImage(
                    url: mainImageUrl,
                    isMain: true,
                    productId: product.Id
                ));
            }

            // 3. Ajouter les images secondaires
            if (galleryUrls != null && galleryUrls.Count > 0)
            {
                foreach (var url in galleryUrls)
                {
                    product.AddImages(new ProductImage(
                        url: url,
                        isMain: false,
                        productId: product.Id
                    ));
                }
            }

            // 4. Ajouter les variantes
            if (dto.Variants != null && dto.Variants.Count > 0)
            {
                foreach (var variantDto in dto.Variants)
                {
                    product.AddVariants(new ProductVariant(
                        name: variantDto.Name,
                        type: "",
                        additionalPrice: variantDto.AdditionalPrice,
                        variantStock: variantDto.VariantStock,
                        productId: product.Id
                    ));
                }
            }

            // 5. Sauvegarder le produit avec toutes ses relations
            await _productRepo.AddAsync(product);

            return product;
        }
        public async Task<Product> UpdateAsync(Guid id, CreateProductDTO dto, string? mainImageUrl, List<string>? galleryUrls)
        {
            var product = await _productRepo.GetByIdAsync(id)
                ?? throw new Exception("Product not found");

         product.Update(
        title: dto.Title,
        description: dto.Description,
        price: dto.Price,
        imageUrl: mainImageUrl,
        stock: dto.Stock,
        discountPercentage: dto.DiscountPercentage,
  discountStartDate: DateTime.SpecifyKind(dto.DiscountStartDate ?? DateTime.UtcNow, DateTimeKind.Utc),
                discountEndDate: DateTime.SpecifyKind(dto.DiscountEndDate ?? DateTime.UtcNow, DateTimeKind.Utc),
        deliveryPriceMaison: dto.DeliveryPriceMaison,
        deliveryPriceBureau: dto.DeliveryPriceBureau,
        categoryId: dto.CategoryId
    );
            await _productRepo.UpdateAsync(product);


            // 4. 🔥 MANUELLEMENT : Gestion des images
            if (galleryUrls != null && galleryUrls.Count!=0)
            {
                // Rechercher et supprimer les anciennes images
                await _productRepo.RemoveAllImagesAsync(product.Id);
                //Add main images 
                if (mainImageUrl != null)
                {
                    await _productRepo.AddImageAsync(new ProductImage(
                            mainImageUrl,
                            true,
                           product.Id
                       ));
                }
                // Créer les nouvelles images
                foreach (var url in galleryUrls)
                {
                    await _productRepo.AddImageAsync(new ProductImage(
                        url, 
                        false,
                       product.Id
                    ));
                }
            }

            if (dto.Variants != null)
            {
                // Rechercher et supprimer les anciennes variantes
                await _variantRepo.RemoveAllVariantsAsync(product.Id);

                // Créer les nouvelles variantes
                foreach (var variantDto in dto.Variants)
                {
                    await _variantRepo.AddAsync(new ProductVariant(
                        variantDto.Name,
                        "",
                        variantDto.AdditionalPrice,
                        variantDto.VariantStock,
                        product.Id
                    ));
                }
            }

            return product;
        
        }


        public async Task<IEnumerable<ProductDTO>> GetBestSellersAsync()
        {
            // 🔹 Requête via le repository
            var bestSellers = await _productRepo.GetBestSellersAsync();

            // 🔹 Conversion en DTO
            return bestSellers.Select(p => new ProductDTO
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                SalesCount = p.SalesCount
            });
        }

        public async Task<bool> DeleteImageAsync(Guid productId, Guid imageId)
        {
            var product = await _productRepo.GetByIdWithGalleryAsync(productId);
            if (product == null) return false;

            // 🔹 CORRECTION : Utiliser le repository pour supprimer l'image
            // Puisque Gallery est en read-only, on ne peut pas utiliser Remove directement
            await _productRepo.RemoveImageAsync(imageId);
            return true;
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _productRepo.GetByIdAsync(id) ?? throw new Exception("Product not found");
            await _productRepo.DeleteAsync(product);
        }

        public async Task<PagedResult<Product>> GetPagedProductsAsync(PaginationParams pagination, string userId)
        {
            var query = _productRepo.GetAllProductWithDetails();

            // 🧩 1. Récupérer les rôles de l'utilisateur connecté
            var roles = await _userRepo.GetRolesAsync(userId);

            // 👤 2. Filtrer si ce n'est PAS un SuperAdmin
            if (roles == null || !roles.Contains("SuperAdmin"))
            {
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    query = query.Where(p => p.UserId == userId);
                }
            }

            // 🔍 3. Recherche
            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(p =>
                    p.Title.Contains(pagination.Search) ||
                    (p.Description != null && p.Description.Contains(pagination.Search)) ||
                    p.Category.Name.Contains(pagination.Search)
                );
            }

            // 🔽 4. Tri
            query = pagination.Sort == "asc"
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt);

            // 📄 5. Pagination
            return await query.ToPagedResultAsync(pagination.Page, pagination.PageSize);
        }

        public async Task<PagedResult<Product>> GetPublicPagedProductsAsync(PaginationParams pagination, Guid? categoryId = null)
        {
            var query = _productRepo.GetAllProductWithDetails();

            // 🔍 Recherche
            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(p =>
                    p.Title.Contains(pagination.Search) ||
                    (p.Description != null && p.Description.Contains(pagination.Search)) ||
                    p.Category.Name.Contains(pagination.Search)
                );
            }

            // 🏷️ Filtrer par catégorie si elle est fournie
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // 🔽 Tri
            query = pagination.Sort == "asc"
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt);

            // 📄 Pagination
            return await query.ToPagedResultAsync(pagination.Page, pagination.PageSize);
        }

        // 🔹 MÉTHODE ADDITIONNELLE : Ajouter une image à un produit existant
        public async Task<bool> AddImageToProductAsync(Guid productId, string imageUrl, bool isMain = false)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null) return false;

            //product.AddImage(imageUrl, isMain);
            await _productRepo.UpdateAsync(product);
            return true;
        }

        // 🔹 MÉTHODE ADDITIONNELLE : Vider toutes les images d'un produit
        public async Task<bool> ClearProductImagesAsync(Guid productId)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null) return false;

          //  product.ClearImages();
            await _productRepo.UpdateAsync(product);
            return true;
        }
    }
}