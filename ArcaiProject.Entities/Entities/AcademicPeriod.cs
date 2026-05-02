using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Entities.Entities
{
    /// <summary>
    /// Akademik dönem (2023-2024 Fall, vb.)
    /// </summary>
    public class AcademicPeriod
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PeriodName { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        // Navigation Properties
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}

