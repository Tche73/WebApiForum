using System.ComponentModel.DataAnnotations;

namespace WebApiForum.Dto_s
{
    public class ReponseDetailsDto
    {
        public string MessageParent { get; set; }
        public string Contenu { get; set; }
        public DateTime DatePublication { get; set; }
        public int NombreLikes { get; set; }
    }
}
