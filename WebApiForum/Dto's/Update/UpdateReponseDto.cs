using System.ComponentModel.DataAnnotations;

namespace WebApiForum.Dto_s
{
    public class UpdateReponseDto
    {
        [Required]
        [StringLength(500)]
        public string Contenu { get; set; }
    }
}
