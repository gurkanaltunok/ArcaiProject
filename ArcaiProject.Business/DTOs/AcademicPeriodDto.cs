namespace ArcaiProject.Business.DTOs
{
    public class AcademicPeriodDto
    {
        public int Id { get; set; }
        public string PeriodName { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
