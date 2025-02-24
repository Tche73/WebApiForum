using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApiForum.Models
{
    public class Reponse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Contenu { get; set; }

        [Required]
        public DateTime DatePublication { get; set; } = DateTime.Now;

        // Relation avec le message
        [Required]
        public int MessageId { get; set; }

        [Required]
        public int? UserId { get; set; }

        [JsonIgnore]
        public virtual Message Message { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
