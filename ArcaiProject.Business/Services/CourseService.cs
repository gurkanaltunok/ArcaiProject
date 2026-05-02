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
    public class CourseService : ICourseService
    {
        private readonly ArcaiDbContext _context;
        private readonly IMapper _mapper;

        public CourseService(ArcaiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedList<CourseDto>> GetAllCoursesAsync(PagingParameters pagingParameters)
        {
            var query = _context.Courses
                .AsNoTracking()
                .OrderBy(c => c.CourseCode);

            var dtoQuery = query.ProjectTo<CourseDto>(_mapper.ConfigurationProvider);

            return await PagedList<CourseDto>.CreateAsync(
                dtoQuery,
                pagingParameters.PageNumber,
                pagingParameters.PageSize
            );
        }

        public async Task<CourseDto?> GetCourseByIdAsync(int id)
        {
            var entity = await _context.Courses.FindAsync(id);
            return entity == null ? null : _mapper.Map<CourseDto>(entity);
        }

        public async Task<CourseDto> AddCourseAsync(CreateCourseDto createDto)
        {
            var exists = await _context.Courses.AnyAsync(x => x.CourseCode == createDto.CourseCode);
            if (exists)
            {
                throw new InvalidOperationException("A Course with the same CourseCode already exists.");
            }

            var entity = _mapper.Map<Course>(createDto);
            _context.Courses.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<CourseDto>(entity);
        }

        public async Task<CourseDto?> UpdateCourseAsync(int id, UpdateCourseDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return null;
            }

            var duplicate = await _context.Courses.AnyAsync(x => x.CourseCode == updateDto.CourseCode && x.Id != id);
            if (duplicate)
            {
                throw new InvalidOperationException("Another Course with the same CourseCode already exists.");
            }

            var entity = await _context.Courses.FindAsync(id);
            if (entity == null)
            {
                return null;
            }

            _mapper.Map(updateDto, entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return _mapper.Map<CourseDto>(entity);
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var entity = await _context.Courses.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _context.Courses.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
