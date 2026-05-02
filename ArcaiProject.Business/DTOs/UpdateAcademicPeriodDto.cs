using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Business.DTOs
{
    public class UpdateAcademicPeriodDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PeriodName { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
