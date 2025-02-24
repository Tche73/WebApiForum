using System.ComponentModel.DataAnnotations;

namespace WebApiForum.Dto_s
{
    public class CreateReponseDto
    {
        [Required]
        public int MessageId { get; set; }

        [Required]
        [StringLength(500)]
        public string Contenu { get; set; }

        [Required]
        public int UserId { get; set; }

    }
}
