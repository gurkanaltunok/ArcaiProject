using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Business.DTOs
{
    public class CreateLocationDto
    {
        [Required]
        [StringLength(150)]
        public string FriendlyName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Room { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Cabinet { get; set; }

        [StringLength(50)]
        public string? Shelf { get; set; }
    }
}
