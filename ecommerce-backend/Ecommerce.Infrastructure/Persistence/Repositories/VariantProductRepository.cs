using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories
{
    public class VariantProductRepository : IVariantProductRepository
    {
        private readonly AppDbContext _context;

        public VariantProductRepository(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 LECTURES AVEC AsNoTracking() pour éviter les conflits
        public async Task RemoveAllVariantsAsync(Guid productId)
        {
            var variants = await _context.ProductVariants
                .Where(v => v.ProductId == productId)
                .ToListAsync();

            _context.ProductVariants.RemoveRange(variants);
            await _context.SaveChangesAsync(); // ✅ Sauvegarde immédiate
        }

        public async Task AddAsync(ProductVariant variant)
        {
            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync(); // ✅ Sauvegarde immédiate
        }
    }
}