using System.ComponentModel.DataAnnotations;

namespace gestionPharmacieApp.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "L'adresse email est requise.")]
        [EmailAddress(ErrorMessage = "L'adresse email est invalide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit comporter au moins 6 caractères.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
