using Ecommerce.Domain.Entities;
using Ecommerce.Domain.DTOs.Products;


namespace Ecommerce.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);

        // Gestion des images
        Task AddImageAsync(ProductImage image);
        Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(Guid productId);
        Task<IEnumerable<Product>> GetBestSellersAsync();

        Task<Product?> GetByIdWithGalleryAsync(Guid id);

        IQueryable<Product> GetAllProductWithDetails();
        Task<Product?> GetByIdForUpdateAsync(Guid id);


        Task RemoveImageAsync(Guid imageId);
        Task RemoveAllImagesAsync(Guid productId);


    }

}
