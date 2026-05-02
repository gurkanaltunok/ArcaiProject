using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Helpers;

namespace ArcaiProject.Business.Interfaces
{
    public interface IDocumentService
    {
        Task<PagedList<DocumentDto>> GetAllDocumentsAsync(PagingParameters pagingParameters);
        Task<DocumentDto?> GetDocumentByIdAsync(int id);
        Task<DocumentDto> AddDocumentAsync(CreateDocumentDto createDto, int adminId);
        Task<DocumentDto?> UpdateDocumentAsync(int id, UpdateDocumentDto updateDto);
        Task<bool> DeleteDocumentAsync(int id);
    }
}
