using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Helpers;

namespace ArcaiProject.Business.Interfaces
{
    /// <summary>
    /// Belge türü yönetim servisi interface'i
    /// </summary>
    public interface IDocumentTypeService
    {
        /// <summary>
        /// Tüm belge türlerini getirir
        /// </summary>
        Task<PagedList<DocumentTypeDto>> GetAllDocumentTypesAsync(PagingParameters pagingParameters);

        /// <summary>
        /// Belirtilen ID'ye sahip belge türünü getirir
        /// </summary>
        Task<DocumentTypeDto?> GetDocumentTypeByIdAsync(int id);

        /// <summary>
        /// Yeni bir belge türü ekler
        /// </summary>
        Task<DocumentTypeDto> AddDocumentTypeAsync(CreateDocumentTypeDto createDto);

        /// <summary>
        /// Mevcut bir belge türünü günceller
        /// </summary>
        Task<DocumentTypeDto?> UpdateDocumentTypeAsync(int id, UpdateDocumentTypeDto updateDto);

        /// <summary>
        /// Belirtilen ID'ye sahip belge türünü siler
        /// </summary>
        Task<bool> DeleteDocumentTypeAsync(int id);
    }
}

