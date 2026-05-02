using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Business.DTOs
{
    public class CreateCourseDto
    {
        [Required]
        [StringLength(20)]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CourseName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Department { get; set; }
    }
}
