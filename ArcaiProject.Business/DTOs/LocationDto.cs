namespace ArcaiProject.Business.DTOs
{
    public class LocationDto
    {
        public int Id { get; set; }
        public string FriendlyName { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string? Cabinet { get; set; }
        public string? Shelf { get; set; }
    }
}
