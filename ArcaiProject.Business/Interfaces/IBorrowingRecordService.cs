using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Helpers;

namespace ArcaiProject.Business.Interfaces
{
    public interface IBorrowingRecordService
    {
        // Professor
        Task<BorrowingRecordDto> RequestDocumentAsync(CreateBorrowingRequestDto createDto, int professorId);
        Task<PagedList<BorrowingRecordDto>> GetMyRequestsAsync(int professorId, PagingParameters pagingParameters);

        // Admin
        Task<BorrowingRecordDto?> ApproveRequestAsync(int recordId, int adminId);
        Task<BorrowingRecordDto?> RejectRequestAsync(int recordId, int adminId);
        Task<BorrowingRecordDto?> CheckoutDocumentAsync(int recordId, int adminId);
        Task<BorrowingRecordDto?> ReturnDocumentAsync(int recordId, int adminId);
        Task<PagedList<BorrowingRecordDto>> GetAllPendingRequestsAsync(PagingParameters pagingParameters);
        Task<PagedList<BorrowingRecordDto>> GetAllApprovedRequestsAsync(PagingParameters pagingParameters);
        Task<PagedList<BorrowingRecordDto>> GetAllBorrowedDocumentsAsync(PagingParameters pagingParameters);
        Task<PagedList<BorrowingRecordDto>> GetAllBorrowingRecordsAsync(PagingParameters pagingParameters);

        // Common
        Task<BorrowingRecordDto?> GetRecordByIdAsync(int id);
    }
}
