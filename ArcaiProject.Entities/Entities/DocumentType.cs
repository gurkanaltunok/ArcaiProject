using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Entities.Entities
{
    /// <summary>
    /// Belge türü (Sınav Kağıdı, Ödev, vb.)
    /// </summary>
    public class DocumentType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}

