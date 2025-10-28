
using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Domain.Interfaces;

    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<ApplicationUser?> GetByIdAsync(string id);

        Task<IdentityResult> UpdateAsync(ApplicationUser user);
  

        Task<IdentityResult> AddAsync(ApplicationUser user, string password);
        Task AddToRoleAsync(ApplicationUser user, string role);
        Task RemoveFromRoleAsync(ApplicationUser user, string role);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task DeleteAsync(ApplicationUser user);
        Task<ApplicationUser?> FindByEmailOrPhoneAsync(string email, string phone);

        IQueryable<ApplicationUser> GetAllUsersWithDetails();
        Task<List<string>> GetRolesAsync(string userId);


}


