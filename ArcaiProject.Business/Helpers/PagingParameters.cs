namespace ArcaiProject.Business.Helpers
{
    public class PagingParameters
    {
        private const int MaxPageSize = 50;
        
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        // Filter parameters for Documents
        public int? DocumentTypeId { get; set; }
        public int? AcademicPeriodId { get; set; }
        public int? LocationId { get; set; }
        public int? CourseId { get; set; }
        public string? Status { get; set; }
        public int? TagId { get; set; }
        public string? SearchTitle { get; set; }

        // Filter parameters for Borrowing Records
        public int? RequesterUserId { get; set; }
        public int? DocumentTypeIdForBorrowing { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? BorrowingStatus { get; set; }
    }
}

