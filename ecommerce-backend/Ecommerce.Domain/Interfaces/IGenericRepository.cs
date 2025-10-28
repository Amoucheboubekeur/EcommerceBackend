

namespace Ecommerce.Domain.Interfaces;

    public interface IGenericRepository<T> where T : class
    {
    
        IQueryable<T> GetAll();

        // 👇 Ne retourne pas PagedResult ici !
        Task<List<T>> GetPagedAsync(int page, int pageSize);
    }

