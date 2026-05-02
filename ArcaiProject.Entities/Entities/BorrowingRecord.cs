using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ArcaiProject.Entities.Enums;

namespace ArcaiProject.Entities.Entities
{
    /// <summary>
    /// Belge ödünç alma kaydı
    /// </summary>
    public class BorrowingRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public BorrowingRecordStatus Status { get; set; } = BorrowingRecordStatus.Pending;

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovalDate { get; set; }

        public DateTime? CheckoutDate { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        // Foreign Keys
        [Required]
        [ForeignKey(nameof(Document))]
        public int DocumentId { get; set; }

        [Required]
        [ForeignKey(nameof(RequesterUser))]
        public int RequesterUserId { get; set; }

        [ForeignKey(nameof(ApproverUser))]
        public int? ApproverUserId { get; set; }

        // Navigation Properties
        public virtual Document Document { get; set; } = null!;
        public virtual User RequesterUser { get; set; } = null!;
        public virtual User? ApproverUser { get; set; }
    }
}

