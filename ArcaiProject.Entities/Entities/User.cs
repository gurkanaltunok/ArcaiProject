using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArcaiProject.Entities.Entities
{
    /// <summary>
    /// Sistem kullanıcısı (Admin veya Professor)
    /// </summary>
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty; // "Admin" veya "Professor"

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        /// <summary>
        /// Bu kullanıcının sisteme eklediği belgeler
        /// </summary>
        public virtual ICollection<Document> DocumentsAdded { get; set; } = new List<Document>();

        /// <summary>
        /// Bu kullanıcının talep ettiği ödünç alma kayıtları
        /// </summary>
        public virtual ICollection<BorrowingRecord> RequestedRecords { get; set; } = new List<BorrowingRecord>();

        /// <summary>
        /// Bu kullanıcının onayladığı ödünç alma kayıtları
        /// </summary>
        public virtual ICollection<BorrowingRecord> ApprovedRecords { get; set; } = new List<BorrowingRecord>();
    }
}

