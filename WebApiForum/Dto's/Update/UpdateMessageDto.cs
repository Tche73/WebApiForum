using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebApiForum.Models;

namespace WebApiForum.Dto_s
{
    public class UpdateMessageDto
    {
        [Required]  // Champ obligatoire
        [StringLength(500)]  // Longueur maximale du message
        public string Contenu { get; set; }

    }
}
