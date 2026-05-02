using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Entities.Entities
{
    /// <summary>
    /// Belge etiketi (kategorilendirme için)
    /// </summary>
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        // Navigation Properties
        public virtual ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();
    }
}

