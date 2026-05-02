using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Business.DTOs
{
    public class CreateBorrowingRequestDto
    {
        [Required]
        public int DocumentId { get; set; }

        [Required]
        [FutureDate(ErrorMessage = "Due date must be in the future.")]
        public DateTime DueDate { get; set; }
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime > DateTime.UtcNow;
            }
            return false;
        }
    }
}
