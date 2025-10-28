
using Ecommerce.Application.Common.Pagination;
using Ecommerce.Domain.DTOs.Categories;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category> CreateAsync(CreateCategoryDTO dto);
        Task<Category> UpdateAsync(Guid id, CreateCategoryDTO dto);
        Task DeleteAsync(Guid id);
        Task<PagedResult<Category>> GetPagedCategoriesAsync(PaginationParams pagination);

    }
}
