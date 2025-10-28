using Ecommerce.Domain.Interfaces;
using Ecommerce.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly AppDbContext _context;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<T> GetAll() => _context.Set<T>();

    public async Task<List<T>> GetPagedAsync(int page, int pageSize)
    {
        return await _context.Set<T>()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
