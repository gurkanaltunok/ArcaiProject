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
    public class AcademicPeriodService : IAcademicPeriodService
    {
        private readonly ArcaiDbContext _context;
        private readonly IMapper _mapper;

        public AcademicPeriodService(ArcaiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedList<AcademicPeriodDto>> GetAllAcademicPeriodsAsync(PagingParameters pagingParameters)
        {
            var query = _context.AcademicPeriods
                .AsNoTracking()
                .OrderBy(ap => ap.PeriodName);

            var dtoQuery = query.ProjectTo<AcademicPeriodDto>(_mapper.ConfigurationProvider);

            return await PagedList<AcademicPeriodDto>.CreateAsync(
                dtoQuery,
                pagingParameters.PageNumber,
                pagingParameters.PageSize
            );
        }

        public async Task<AcademicPeriodDto?> GetAcademicPeriodByIdAsync(int id)
        {
            var entity = await _context.AcademicPeriods.FindAsync(id);
            return entity == null ? null : _mapper.Map<AcademicPeriodDto>(entity);
        }

        public async Task<AcademicPeriodDto> AddAcademicPeriodAsync(CreateAcademicPeriodDto createDto)
        {
            var exists = await _context.AcademicPeriods.AnyAsync(x => x.PeriodName == createDto.PeriodName);
            if (exists)
            {
                throw new InvalidOperationException("An AcademicPeriod with the same PeriodName already exists.");
            }

            var entity = _mapper.Map<AcademicPeriod>(createDto);
            _context.AcademicPeriods.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<AcademicPeriodDto>(entity);
        }

        public async Task<AcademicPeriodDto?> UpdateAcademicPeriodAsync(int id, UpdateAcademicPeriodDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return null;
            }

            var duplicate = await _context.AcademicPeriods.AnyAsync(x => x.PeriodName == updateDto.PeriodName && x.Id != id);
            if (duplicate)
            {
                throw new InvalidOperationException("Another AcademicPeriod with the same PeriodName already exists.");
            }

            var entity = await _context.AcademicPeriods.FindAsync(id);
            if (entity == null)
            {
                return null;
            }

            _mapper.Map(updateDto, entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return _mapper.Map<AcademicPeriodDto>(entity);
        }

        public async Task<bool> DeleteAcademicPeriodAsync(int id)
        {
            var entity = await _context.AcademicPeriods.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _context.AcademicPeriods.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
