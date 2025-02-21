using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiForum.Models;
public class Message
{
    [Key]  // Définit la clé primaire
    public int Id { get; set; }

    [Required]  // Champ obligatoire
    [StringLength(500)]  // Longueur maximale du message
    public string Contenu { get; set; }

    [Required]
    public DateTime DatePublication { get; set; } = DateTime.Now;

    // Relation avec les réponses (1 message peut avoir plusieurs réponses)
    public virtual ICollection<Reponse> Reponses { get; set; } = new List<Reponse>();
}
