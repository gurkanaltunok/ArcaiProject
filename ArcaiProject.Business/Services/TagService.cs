using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Interfaces;
using ArcaiProject.Business.Helpers;
using ArcaiProject.DataAccess.Context;
using ArcaiProject.Entities.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ArcaiProject.Business.Services
{
    public class TagService : ITagService
    {
        private readonly ArcaiDbContext _context;
        private readonly IMapper _mapper;

        public TagService(ArcaiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedList<TagDto>> GetAllTagsAsync(PagingParameters pagingParameters)
        {
            var query = _context.Tags
                .AsNoTracking()
                .OrderBy(t => t.Name);

            var dtoQuery = query.ProjectTo<TagDto>(_mapper.ConfigurationProvider);

            return await PagedList<TagDto>.CreateAsync(
                dtoQuery,
                pagingParameters.PageNumber,
                pagingParameters.PageSize
            );
        }

        public async Task<TagDto?> GetTagByIdAsync(int id)
        {
            var entity = await _context.Tags.FindAsync(id);
            return entity == null ? null : _mapper.Map<TagDto>(entity);
        }

        public async Task<TagDto> AddTagAsync(CreateTagDto createDto)
        {
            var exists = await _context.Tags.AnyAsync(x => x.Name == createDto.Name);
            if (exists)
            {
                throw new InvalidOperationException("A Tag with the same Name already exists.");
            }

            var entity = _mapper.Map<Tag>(createDto);
            _context.Tags.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<TagDto>(entity);
        }

        public async Task<TagDto?> UpdateTagAsync(int id, UpdateTagDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return null;
            }

            var duplicate = await _context.Tags.AnyAsync(x => x.Name == updateDto.Name && x.Id != id);
            if (duplicate)
            {
                throw new InvalidOperationException("Another Tag with the same Name already exists.");
            }

            var entity = await _context.Tags.FindAsync(id);
            if (entity == null)
            {
                return null;
            }

            _mapper.Map(updateDto, entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return _mapper.Map<TagDto>(entity);
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            var entity = await _context.Tags.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _context.Tags.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
