using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Business.DTOs
{
    public class CreateTagDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
