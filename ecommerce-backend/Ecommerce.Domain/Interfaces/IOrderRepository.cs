using Ecommerce.Domain.Entities;
namespace Ecommerce.Domain.Interfaces;
    public interface IOrderRepository
    {
        // ✅ Créer une commande
        Task<Guid> CreateAsync(Order order);

        // ✅ Récupérer une commande par Id et User
        Task<Order?> GetByIdAsync(Guid orderId, string userId);
        Task<Order?> GetByIdAsync(Guid orderId);


        // ✅ Récupérer toutes les commandes d’un utilisateur
        Task<List<Order>> GetAllByUserIdAsync(string userId);

        // ✅ Mettre à jour une commande
        Task UpdateAsync(Order order);

        // ✅ Supprimer une commande
        Task DeleteAsync(Order order);
        Task<List<Order>> Getallorderasync();
        Task<(IEnumerable<Order> Orders, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search = null, string? status = null);

        IQueryable<Order> GetAllOrdersWithDetails();
    }


