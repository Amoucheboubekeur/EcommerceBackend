using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.DTOs.Users
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(50, ErrorMessage = "Le prénom ne peut dépasser 50 caractères")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(50, ErrorMessage = "Le nom ne peut dépasser 50 caractères")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le numéro de téléphone est requis")]
        [Phone(ErrorMessage = "Numéro de téléphone invalide")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'adresse est requise")]
        [StringLength(200, ErrorMessage = "L'adresse ne peut dépasser 200 caractères")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ville est requise")]
        [StringLength(50, ErrorMessage = "La ville ne peut dépasser 50 caractères")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "La wilaya est requise")]
        [StringLength(50, ErrorMessage = "La wilaya ne peut dépasser 50 caractères")]
        public string Wilaya { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le code postal est requis")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Code postal invalide")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit faire entre 6 et 100 caractères")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmation du mot de passe est requise")]
        [Compare("Password", ErrorMessage = "Le mot de passe et sa confirmation ne correspondent pas")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
    }
}
