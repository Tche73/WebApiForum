using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApiForum.Models
{
    [Table("Messages")]
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le contenu est obligatoire")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Le contenu doit avoir entre 1 et 500 caractères")]
        public string Contenu { get; set; }

        [Required(ErrorMessage = "La date de publication est obligatoire")]
        [DataType(DataType.DateTime)]
        public DateTime DatePublication { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "L'utilisateur est obligatoire")]
        public int UserId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public ICollection<Reponse> Reponses { get; set; } = new List<Reponse>();
    }
}
