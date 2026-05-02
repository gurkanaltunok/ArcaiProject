using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Interfaces;
using ArcaiProject.Business.Helpers;
using ArcaiProject.DataAccess.Context;
using ArcaiProject.Entities.Entities;
using ArcaiProject.Entities.Enums;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ArcaiProject.Business.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ArcaiDbContext _context;
        private readonly IMapper _mapper;

        public DocumentService(ArcaiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedList<DocumentDto>> GetAllDocumentsAsync(PagingParameters pagingParameters)
        {
            // Debug: Log received parameters
            System.Diagnostics.Debug.WriteLine($"DocumentTypeId: {pagingParameters.DocumentTypeId}, LocationId: {pagingParameters.LocationId}, CourseId: {pagingParameters.CourseId}, Status: {pagingParameters.Status}, SearchTitle: {pagingParameters.SearchTitle}");

            // 1. Veritabanı sorgusunu (IQueryable) hazırla
            var query = _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.Location)
                .Include(d => d.Course)
                .Include(d => d.AcademicPeriod)
                .Include(d => d.AddedByUser)
                .Include(d => d.DocumentTags)
                    .ThenInclude(dt => dt.Tag)
                .Where(d => d.IsDeleted == false) // Only get non-deleted documents
                .AsNoTracking();

            // Apply filters
            if (pagingParameters.DocumentTypeId.HasValue && pagingParameters.DocumentTypeId.Value > 0)
            {
                query = query.Where(d => d.DocumentTypeId == pagingParameters.DocumentTypeId.Value);
                System.Diagnostics.Debug.WriteLine($"Applied DocumentTypeId filter: {pagingParameters.DocumentTypeId.Value}");
            }

            if (pagingParameters.AcademicPeriodId.HasValue && pagingParameters.AcademicPeriodId.Value > 0)
            {
                query = query.Where(d => d.AcademicPeriodId == pagingParameters.AcademicPeriodId.Value);
            }

            if (pagingParameters.LocationId.HasValue && pagingParameters.LocationId.Value > 0)
            {
                query = query.Where(d => d.LocationId == pagingParameters.LocationId.Value);
            }

            if (pagingParameters.CourseId.HasValue && pagingParameters.CourseId.Value > 0)
            {
                query = query.Where(d => d.CourseId == pagingParameters.CourseId.Value);
            }

            if (!string.IsNullOrWhiteSpace(pagingParameters.Status))
            {
                if (Enum.TryParse<DocumentStatus>(pagingParameters.Status, out var status))
                {
                    query = query.Where(d => d.Status == status);
                }
            }

            if (pagingParameters.TagId.HasValue && pagingParameters.TagId.Value > 0)
            {
                query = query.Where(d => d.DocumentTags.Any(dt => dt.TagId == pagingParameters.TagId.Value));
            }

            if (!string.IsNullOrWhiteSpace(pagingParameters.SearchTitle))
            {
                query = query.Where(d => d.Title.Contains(pagingParameters.SearchTitle));
            }

            query = query.OrderByDescending(d => d.CreatedAt); // Örnek sıralama

            // Debug: Count before projection
            var countBeforeProjection = await query.CountAsync();
            System.Diagnostics.Debug.WriteLine($"Count before projection and pagination: {countBeforeProjection}");

            // 2. AutoMapper'ın Projeksiyonunu kullanarak DTO'ya dönüştür
            // Bu, veritabanından sadece DTO'nun ihtiyaç duyduğu alanları çeker (daha performanslı)
            var dtoQuery = query.ProjectTo<DocumentDto>(_mapper.ConfigurationProvider);

            // 3. PagedList helper'ını kullanarak sorguyu çalıştır ve PagedList oluştur
            var result = await PagedList<DocumentDto>.CreateAsync(
                dtoQuery, 
                pagingParameters.PageNumber, 
                pagingParameters.PageSize
            );
            
            // Debug: Log result
            System.Diagnostics.Debug.WriteLine($"Result count: {result.Count}, Total: {result.Metadata.TotalCount}");
            
            return result;
        }

        public async Task<DocumentDto?> GetDocumentByIdAsync(int id)
        {
            var document = await _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.Location)
                .Include(d => d.Course)
                .Include(d => d.AcademicPeriod)
                .Include(d => d.AddedByUser)
                .Include(d => d.DocumentTags).ThenInclude(dt => dt.Tag)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted == false);

            return document == null ? null : _mapper.Map<DocumentDto>(document);
        }

        public async Task<DocumentDto> AddDocumentAsync(CreateDocumentDto createDto, int adminId)
        {
            var entity = _mapper.Map<Document>(createDto);
            entity.AddedByUserId = adminId;
            entity.Status = DocumentStatus.Available;
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsDeleted = false; // New documents are not deleted by default

            if (createDto.TagIds != null && createDto.TagIds.Count > 0)
            {
                foreach (var tagId in createDto.TagIds.Distinct())
                {
                    var exists = await _context.Tags.AnyAsync(t => t.Id == tagId);
                    if (exists)
                    {
                        entity.DocumentTags.Add(new DocumentTag { TagId = tagId });
                    }
                }
            }

            _context.Documents.Add(entity);
            await _context.SaveChangesAsync();

            var dto = await GetDocumentByIdAsync(entity.Id);
            return dto!;
        }

        public async Task<DocumentDto?> UpdateDocumentAsync(int id, UpdateDocumentDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return null;
            }

            var entity = await _context.Documents
                .Include(d => d.DocumentTags)
                .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted == false);
            if (entity == null)
            {
                return null;
            }

            _mapper.Map(updateDto, entity);

            // Update many-to-many tags
            entity.DocumentTags.Clear();
            if (updateDto.TagIds != null && updateDto.TagIds.Count > 0)
            {
                foreach (var tagId in updateDto.TagIds.Distinct())
                {
                    var exists = await _context.Tags.AnyAsync(t => t.Id == tagId);
                    if (exists)
                    {
                        entity.DocumentTags.Add(new DocumentTag { TagId = tagId, DocumentId = entity.Id });
                    }
                }
            }

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var dto = await GetDocumentByIdAsync(entity.Id);
            return dto;
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            var entity = await _context.Documents.FindAsync(id);
            if (entity == null || entity.IsDeleted)
            {
                return false;
            }

            // Soft delete: Set IsDeleted to true instead of removing
            entity.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
