using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArcaiProject.Entities.Entities
{
    /// <summary>
    /// Belge ve Etiket arasındaki çoktan çoğa ilişki tablosu
    /// </summary>
    public class DocumentTag
    {
        [Required]
        [ForeignKey(nameof(Document))]
        public int DocumentId { get; set; }

        [Required]
        [ForeignKey(nameof(Tag))]
        public int TagId { get; set; }

        // Navigation Properties
        public virtual Document Document { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}

