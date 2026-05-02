namespace ArcaiProject.Business.DTOs
{
    public class BorrowingRecordDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? CheckoutDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public DocumentDto? Document { get; set; }
        public UserDto? RequesterUser { get; set; }
        public UserDto? ApproverUser { get; set; }
    }
}
