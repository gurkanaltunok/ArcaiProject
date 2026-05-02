using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Entities.Entities
{
    /// <summary>
    /// Ders bilgisi
    /// </summary>
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CourseName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Department { get; set; }

        // Navigation Properties
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}

