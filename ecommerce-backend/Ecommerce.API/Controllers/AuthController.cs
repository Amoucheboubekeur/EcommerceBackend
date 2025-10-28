using Ecommerce.Domain.DTOs.Users;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserRepository userRepo, JwtService jwtService, PasswordHasher hasher) : ControllerBase
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly JwtService _jwtService = jwtService;
    private readonly PasswordHasher _hasher = hasher;

    [HttpPost("register/user")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        // Vérifier si l'email existe déjà
        var userExists = await _userRepo.GetByEmailAsync(dto.Email);
        if (userExists != null)
            return BadRequest("User already exists");

        if (dto.Password != dto.ConfirmPassword)
            return BadRequest("Passwords do not match");

        var user = new ApplicationUser
        {
            UserName = dto.Email, // obligatoire pour Identity
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Address = dto.Address,
            Commune = dto.City,
            Wilaya = dto.Wilaya,
            PostalCode = dto.PostalCode,
            IsGuest = false
        };

        // ⚡ Créer l'utilisateur avec hashage du mot de passe
        var result = await _userRepo.AddAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Ajouter un rôle par défaut
        await _userRepo.AddToRoleAsync(user, "User");

        // ✅ Attendre le résultat de la génération du token
        var token = await _jwtService.GenerateToken(user);

        // ✅ Retourner la réponse complète
        return Ok(new AuthResponseDTO
        {
            Email = user.Email,
            Token = token
        });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var user = await _userRepo.GetByEmailAsync(dto.Email);
        if (user == null || !_hasher.Verify(user.PasswordHash, dto.Password))
            return Unauthorized("Invalid credentials");

        var token = await _jwtService.GenerateToken(user);

        return Ok(new AuthResponseDTO { Email = user.Email, Token = token });
    }
}
