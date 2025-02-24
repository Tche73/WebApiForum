using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiForum.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReponseId { get; set; }
        [Required]
        public int UserId { get; set; }  // L'utilisateur qui a liké la réponse


        public virtual Reponse Reponse { get; set; }
        public virtual User User { get; set; }

    }
}
