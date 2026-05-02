using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Entities.Entities
{
    /// <summary>
    /// Belgenin fiziksel konumu (Oda, Dolap, Raf)
    /// </summary>
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string FriendlyName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Room { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Cabinet { get; set; }

        [StringLength(50)]
        public string? Shelf { get; set; }

        // Navigation Properties
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}

