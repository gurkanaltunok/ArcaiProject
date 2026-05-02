using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ArcaiProject.Entities.Enums;

namespace ArcaiProject.Entities.Entities
{
    /// <summary>
    /// Fiziksel belge kaydı
    /// </summary>
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DocumentStatus Status { get; set; } = DocumentStatus.Available;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public bool IsDeleted { get; set; } = false;

        // Foreign Keys
        [Required]
        [ForeignKey(nameof(DocumentType))]
        public int DocumentTypeId { get; set; }

        [Required]
        [ForeignKey(nameof(Location))]
        public int LocationId { get; set; }

        [Required]
        [ForeignKey(nameof(AddedByUser))]
        public int AddedByUserId { get; set; }

        [ForeignKey(nameof(Course))]
        public int? CourseId { get; set; }

        [ForeignKey(nameof(AcademicPeriod))]
        public int? AcademicPeriodId { get; set; }

        // Navigation Properties
        public virtual DocumentType DocumentType { get; set; } = null!;
        public virtual Location Location { get; set; } = null!;
        public virtual User AddedByUser { get; set; } = null!;
        public virtual Course? Course { get; set; }
        public virtual AcademicPeriod? AcademicPeriod { get; set; }
        public virtual ICollection<BorrowingRecord> BorrowingRecords { get; set; } = new List<BorrowingRecord>();
        public virtual ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();
    }
}

