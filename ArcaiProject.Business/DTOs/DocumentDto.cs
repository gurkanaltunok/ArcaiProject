using System.Collections.Generic;

namespace ArcaiProject.Business.DTOs
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public DocumentTypeDto? DocumentType { get; set; }
        public LocationDto? Location { get; set; }
        public CourseDto? Course { get; set; }
        public AcademicPeriodDto? AcademicPeriod { get; set; }
        public UserDto? AddedByUser { get; set; }
        public ICollection<TagDto> Tags { get; set; } = new List<TagDto>();
    }
}
