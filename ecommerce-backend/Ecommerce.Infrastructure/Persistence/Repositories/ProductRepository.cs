using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories
{
    public class ProductRepository(AppDbContext context) : IProductRepository
    {
        private readonly AppDbContext _context = context;


        public async Task<Product?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task RemoveAllImagesAsync(Guid productId)
        {
            var images = await _context.ProductImages
                .Where(img => img.ProductId == productId)
                .ToListAsync();

            _context.ProductImages.RemoveRange(images);
            await _context.SaveChangesAsync(); // ✅ Sauvegarde immédiate
        }

        public async Task AddImageAsync(ProductImage image)
        {
            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync(); // ✅ Sauvegarde immédiate
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.AsTracking()
                .Include(p => p.Category)
                .Include(p => p.Gallery)
                .Include(p => p.User)
                 .Include(p => p.Variants)
                .ToListAsync();
        }
        public IQueryable<Product> GetAllProductWithDetails()
        {
            return _context.Products
                .Include(p => p.Category)        // si le produit a une catégorie
                .Include(p => p.Gallery)         
                .Include(p => p.User)  
                .Include(p=> p.Variants)
                .OrderByDescending(p => p.CreatedAt)
                .AsQueryable();
        }

        public async Task<IEnumerable<Product>> GetBestSellersAsync()
        {
            return await _context.Products
                .OrderByDescending(p => p.SalesCount)
                .Take(2)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .Include(p => p.Gallery)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        // Ajoutez cette méthode dans votre Repository pour les opérations de modification
        //public async Task<Product?> GetByIdForUpdateAsync(Guid id)
        //{
        //    return await _context.Products
        //        //.Include(p => p.Gallery)
        //        //.Include(p => p.Variants)
        //        .FirstOrDefaultAsync(p => p.Id == id);
        //}
        public async Task AddAsync(Product product)
        {
           // product.DiscountStartDate = DateTime.SpecifyKind(product.DiscountStartDate ?? DateTime.UtcNow, DateTimeKind.Utc);
         ///   product.DiscountEndDate = DateTime.SpecifyKind(product.DiscountEndDate ?? DateTime.UtcNow, DateTimeKind.Utc);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
        public async Task<Product?> GetByIdWithGalleryAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Gallery)
                .FirstOrDefaultAsync(p => p.Id == id);
        }



        public async Task UpdateAsync(Product product)
        {       _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        // ---- Gestion des images ----
        //public async Task AddImageAsync(ProductImage image)
        //{
        //    await _context.ProductImages.AddAsync(image);
        //    await _context.SaveChangesAsync();
        //}

        public async Task RemoveImageAsync(Guid imageId)
        {
            // 🔹 CORRECTION : Chercher d'abord l'image par son ID
            var image = await _context.ProductImages
                .FirstOrDefaultAsync(img => img.Id == imageId);

            if (image != null)
            {
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(Guid productId)
        {
            return await _context.ProductImages
                .Where(i => i.ProductId == productId)
                .ToListAsync();
        }
    }
}
