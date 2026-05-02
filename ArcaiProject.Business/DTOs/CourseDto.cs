namespace ArcaiProject.Business.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? Department { get; set; }
    }
}
