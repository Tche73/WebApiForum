using System.ComponentModel.DataAnnotations;

namespace WebApiForum.Dto_s
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Le nom d'utilisateur doit avoir entre 3 et 50 caractères")]
        public string Username { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Veuillez entrer une adresse email valide")]
        [StringLength(256, ErrorMessage = "L'email ne peut pas dépasser 256 caractères")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit avoir au moins 6 caractères")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
