using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class UserRepository(UserManager<ApplicationUser> userManager) : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
    public IQueryable<ApplicationUser> GetAllUsersWithDetails()
    {
        return _userManager.Users
            .OrderByDescending(u => u.FirstName)
            .AsQueryable();
    }

    public async Task<ApplicationUser?> FindByEmailOrPhoneAsync(string email, string phone)
    {
        return await _userManager.Users
            .FirstOrDefaultAsync(u => u.Email == email || u.PhoneNumber == phone);
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<IdentityResult> AddAsync(ApplicationUser user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationUser user)
    {
        return await _userManager.UpdateAsync(user);
    }


    public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
    {
        return await _userManager.Users.ToListAsync();
    }

    public async Task AddToRoleAsync(ApplicationUser user, string role)
    {
        await _userManager.AddToRoleAsync(user, role);
    }

    public async Task RemoveFromRoleAsync(ApplicationUser user, string role)
    {
        await _userManager.RemoveFromRoleAsync(user, role);
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }
    public async Task DeleteAsync(ApplicationUser user)
    {
        await _userManager.DeleteAsync(user);
    }
    public async Task<List<string>> GetRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null ? (await _userManager.GetRolesAsync(user)).ToList() : new List<string>();
    }
}
