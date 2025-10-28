using Ecommerce.Domain.Entities;


namespace Ecommerce.Domain.Interfaces
{
    public interface IVariantProductRepository
    {
  Task RemoveAllVariantsAsync(Guid productId);


        Task AddAsync(ProductVariant variant);
   
}
}