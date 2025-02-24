using System.ComponentModel.DataAnnotations;

namespace WebApiForum.Dto_s
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
