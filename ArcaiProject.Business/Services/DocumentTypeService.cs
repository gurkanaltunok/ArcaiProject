using ArcaiProject.Business.Interfaces;
using ArcaiProject.DataAccess.Context;
using ArcaiProject.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace ArcaiProject.Business.Services
{
    /// <summary>
    /// Belge türü yönetim servisi implementasyonu
    /// </summary>
    public class DocumentTypeService : IDocumentTypeService
    {
        private readonly ArcaiDbContext _context;
        private readonly IMapper _mapper;

        public DocumentTypeService(ArcaiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedList<DocumentTypeDto>> GetAllDocumentTypesAsync(PagingParameters pagingParameters)
        {
            var query = _context.DocumentTypes
                .AsNoTracking()
                .OrderBy(dt => dt.Name);

            var dtoQuery = query.ProjectTo<DocumentTypeDto>(_mapper.ConfigurationProvider);

            return await PagedList<DocumentTypeDto>.CreateAsync(
                dtoQuery,
                pagingParameters.PageNumber,
                pagingParameters.PageSize
            );
        }

        public async Task<DocumentTypeDto?> GetDocumentTypeByIdAsync(int id)
        {
            var entity = await _context.DocumentTypes.FindAsync(id);
            return entity == null ? null : _mapper.Map<DocumentTypeDto>(entity);
        }

        public async Task<DocumentTypeDto> AddDocumentTypeAsync(CreateDocumentTypeDto createDto)
        {
            // Unique Name constraint kontrolü
            var existingWithSameName = await _context.DocumentTypes.FirstOrDefaultAsync(dt => dt.Name == createDto.Name);
            if (existingWithSameName != null)
            {
                throw new InvalidOperationException("A DocumentType with the same name already exists.");
            }

            var entity = _mapper.Map<DocumentType>(createDto);
            _context.DocumentTypes.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<DocumentTypeDto>(entity);
        }

        public async Task<DocumentTypeDto?> UpdateDocumentTypeAsync(int id, UpdateDocumentTypeDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return null;
            }

            var existingName = await _context.DocumentTypes.FirstOrDefaultAsync(dt => dt.Name == updateDto.Name && dt.Id != updateDto.Id);
            if (existingName != null)
            {
                throw new InvalidOperationException("Another DocumentType with the same name already exists.");
            }

            var entity = await _context.DocumentTypes.FindAsync(id);
            if (entity == null)
            {
                return null;
            }

            _mapper.Map(updateDto, entity);

            _context.Entry(entity).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return _mapper.Map<DocumentTypeDto>(entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DocumentTypeExists(id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteDocumentTypeAsync(int id)
        {
            var entity = await _context.DocumentTypes.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _context.DocumentTypes.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> DocumentTypeExists(int id)
        {
            return await _context.DocumentTypes.AnyAsync(e => e.Id == id);
        }
    }
}

