using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApiForum.Models
{
    [Table("Reponses")]
    public class Reponse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le contenu de la réponse est obligatoire")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Le contenu doit avoir entre 1 et 500 caractères")]
        public string Contenu { get; set; }

        [Required(ErrorMessage = "La date de publication est obligatoire")]
        [DataType(DataType.DateTime)]
        public DateTime DatePublication { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Le message parent est obligatoire")]
        public int MessageId { get; set; }

        [Required(ErrorMessage = "L'utilisateur est obligatoire")]
        public int UserId { get; set; }

        // Navigation properties
        public Message Message { get; set; }
        public User User { get; set; }
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
