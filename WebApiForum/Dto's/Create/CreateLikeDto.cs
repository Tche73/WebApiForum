using System.ComponentModel.DataAnnotations;

namespace WebApiForum.Dto_s
{
    public class CreateLikeDto
    {
        [Required]
        public int ReponseId { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
