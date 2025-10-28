using Ecommerce.Application.Common.Pagination;
using Ecommerce.Domain.DTOs.Users;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")] // Seul le SuperAdmin peut accéder ici
public class UsersController(IUserRepository userRepo, RoleManager<IdentityRole> roleManager) : ControllerBase
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    // ✅ 1. Obtenir la liste de tous les utilisateurs
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userRepo.GetAllAsync();
        return Ok(users);
    }

    // ✅ 2. Obtenir un utilisateur par ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "Utilisateur introuvable." });
        var roles = await _userRepo.GetRolesAsync(user);
        var userDto = new
        {
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.PhoneNumber,
            user.Address,
            user.Commune,
            user.Wilaya,
            user.PostalCode,
            Role = roles.FirstOrDefault() // On prend le premier rôle s’il y en a un
        };
        return Ok(userDto);
    }

    // ✅ Liste de tous les rôles
    [HttpGet("roles")]
    public IActionResult GetRoles()
    {
        var roles = _roleManager.Roles.Select(r => r.Name).ToList();
        return Ok(roles);
    }

    // ✅ 3. Créer un utilisateur (SuperAdmin peut créer Admin ou User)
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] RegisterDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Vérifie si le rôle existe, sinon le crée
        if (!await _roleManager.RoleExistsAsync(dto.Role))
            await _roleManager.CreateAsync(new IdentityRole(dto.Role));

        // 🔹 Convertir le DTO en ApplicationUser
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Address = dto.Address,
            Commune = dto.City,
            Wilaya = dto.Wilaya,
            PostalCode = dto.PostalCode,
            EmailConfirmed = true // (optionnel, selon ta logique)
        };

        // 🔹 Appeler ton dépôt avec l'utilisateur et le rôle
        var result = await _userRepo.AddAsync(user, dto.Password);


        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userRepo.AddToRoleAsync(user, dto.Role);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = $"Utilisateur créé avec succès avec le rôle '{dto.Role}'." });
    }

    // ✅ 7. Mettre à jour un utilisateur
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] RegisterDTO dto)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "Utilisateur introuvable." });

        // Mise à jour des champs
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.UserName = dto.Email;
        user.PhoneNumber = dto.PhoneNumber;
        user.Address = dto.Address;
        user.Commune = dto.City;
        user.Wilaya = dto.Wilaya;
        user.PostalCode = dto.PostalCode;

        // Sauvegarde
        var result = await _userRepo.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Met à jour le rôle si différent
        var roles = await _userRepo.GetRolesAsync(user);
        if (!roles.Contains(dto.Role))
        {
            foreach (var role in roles)
                await _userRepo.RemoveFromRoleAsync(user, role);

            await _userRepo.AddToRoleAsync(user, dto.Role);
        }

        return Ok(new { message = "Utilisateur mis à jour avec succès." });
    }

    // ✅ 4. Supprimer un utilisateur
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "Utilisateur introuvable." });

        await _userRepo.DeleteAsync(user);
        return Ok(new { message = "Utilisateur supprimé avec succès." });
    }

    // ✅ 5. Assigner un rôle
    [HttpPost("{id}/roles")]
    public async Task<IActionResult> AssignRole(string id, [FromBody][Required] string role)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "Utilisateur introuvable." });

        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole(role));

        await _userRepo.AddToRoleAsync(user, role);
        return Ok(new { message = $"Rôle '{role}' ajouté à l'utilisateur." });
    }

    // ✅ 6. Retirer un rôle
    [HttpDelete("{id}/roles")]
    public async Task<IActionResult> RemoveRole(string id, [FromBody][Required] string role)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "Utilisateur introuvable." });

        await _userRepo.RemoveFromRoleAsync(user, role);
        return Ok(new { message = $"Rôle '{role}' retiré de l'utilisateur." });
    }


    [HttpGet("GetPaged")]
    public async Task<IActionResult> GetPaged([FromQuery] PaginationParams pagination)
    {
        var query = _userRepo.GetAllUsersWithDetails(); // ✅ doit retourner IQueryable<Product>

        // 🔍 Filtrage / Recherche
        if (!string.IsNullOrWhiteSpace(pagination.Search))
        {
            query = query.Where(p =>
                p.FirstName.Contains(pagination.Search) ||
                p.LastName.Contains(pagination.Search) ||
                p.Email.Contains(pagination.Search));
        }

        // 🔽 Tri
        query = pagination.Sort == "asc"
            ? query.OrderBy(p => p.FirstName)
            : query.OrderByDescending(p => p.FirstName);

        // 📄 Pagination
        var result = await query.ToPagedResultAsync(pagination.Page, pagination.PageSize);
        return Ok(result);
    }
}
