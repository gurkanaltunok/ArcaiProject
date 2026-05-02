using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Business.DTOs
{
    public class UpdateTagDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
