using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApiForum.Models;

public class Like
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ReponseId { get; set; }

    [ForeignKey("ReponseId")]
    public virtual Reponse Reponse { get; set; }

    [Required]
    public string UserId { get; set; }  // L'utilisateur qui a liké la réponse
}
