using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Application.Services;

public class PasswordHasher
{
    private readonly IPasswordHasher<string> _hasher = new PasswordHasher<string>();

    public string Hash(string password)
    {
        return _hasher.HashPassword("user", password);
    }

    public bool Verify(string? hashedPassword, string providedPassword)
    {
        if (hashedPassword == null) return false;
        var result = _hasher.VerifyHashedPassword("user", hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }
}
