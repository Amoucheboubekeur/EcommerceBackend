

using Ecommerce.Application.Common.Pagination;
using Ecommerce.Domain.DTOs.Products;
    using Ecommerce.Domain.Entities;

    namespace Ecommerce.Application.Interfaces
    {
        public interface IProductService
        {
            Task<IEnumerable<Product>> GetAllAsync();
            Task<Product?> GetByIdAsync(Guid id);
            Task<Product> CreateAsync(CreateProductDTO dto, string? mainImageUrl, List<string>? galleryUrls,string userId);
            Task DeleteAsync(Guid id);
            Task<Product> UpdateAsync(Guid id, CreateProductDTO dto, string? mainImageUrl, List<string>? galleryUrls);
            Task<IEnumerable<ProductDTO>> GetBestSellersAsync();
            Task<bool> DeleteImageAsync(Guid productId, Guid imageId);
            Task<PagedResult<Product>> GetPagedProductsAsync(PaginationParams pagination,string userId);
           Task<PagedResult<Product>> GetPublicPagedProductsAsync(PaginationParams pagination, Guid? categoryId = null);
    }
}

