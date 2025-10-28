using Ecommerce.Application.Common.Pagination;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.DTOs.Categories;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces;

namespace Ecommerce.Application.Services
{
    public class CategoryService(ICategoryRepository repo) : ICategoryService
    {
        private readonly ICategoryRepository _repo = repo;

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }
        public async Task<PagedResult<Category>> GetPagedCategoriesAsync(PaginationParams pagination)
        {
            var query = _repo.GetAllCategoriesWithDetails();

            // 🔍 Filtrage / Recherche
            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(c =>
                    c.Name.Contains(pagination.Search));
                 
            }


            // 📄 Pagination
            return await query.ToPagedResultAsync(pagination.Page, pagination.PageSize);
        }
        public async Task<Category> CreateAsync(CreateCategoryDTO dto)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
            };

            await _repo.AddAsync(category);
            return category;
        }

        public async Task<Category> UpdateAsync(Guid id, CreateCategoryDTO dto)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null) throw new Exception("Category not found");

            category.Name = dto.Name;

            await _repo.UpdateAsync(category);
            return category;
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null) throw new Exception("Category not found");

            await _repo.DeleteAsync(category);
        }
    }
}
