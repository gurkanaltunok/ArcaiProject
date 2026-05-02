using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Business.DTOs
{
    public class CreateDocumentTypeDto
    {
        [Required(ErrorMessage = "Document type name is required.")]
        [StringLength(100, ErrorMessage = "Document type name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
