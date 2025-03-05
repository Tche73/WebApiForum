using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiForum.Models
{
    public class User : IdentityUser<int>
    {

        [Required]
        [StringLength(50)]
        
        public string Displayname { get; set; }

        // Vous pourriez ajouter d'autres propriétés comme
        // Password (hachée), DateInscription, etc.

        // Relations
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
        public virtual ICollection<Reponse> Reponses { get; set; } = new List<Reponse>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
