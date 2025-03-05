using System.ComponentModel.DataAnnotations;

namespace WebApiForum.Dto_s.Update
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string DisplayName { get; set; }
    }
}
