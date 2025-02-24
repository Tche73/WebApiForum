using System.ComponentModel.DataAnnotations;

namespace WebApiForum.Dto_s
{
    public class CreateMessageDto
    {
        [Required]
        [StringLength(500)]
        public string Contenu { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
