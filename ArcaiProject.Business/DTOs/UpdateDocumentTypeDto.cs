using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Business.DTOs
{
    public class UpdateDocumentTypeDto
    {
        [Required(ErrorMessage = "Document type ID is required.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Document type name is required.")]
        [StringLength(100, ErrorMessage = "Document type name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
