using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Business.DTOs
{
    public class UpdateDocumentDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public int DocumentTypeId { get; set; }

        [Required]
        public int LocationId { get; set; }

        public int? CourseId { get; set; }
        public int? AcademicPeriodId { get; set; }

        public ICollection<int> TagIds { get; set; } = new List<int>();
    }
}
