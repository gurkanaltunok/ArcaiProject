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
    public class LocationService : ILocationService
    {
        private readonly ArcaiDbContext _context;
        private readonly IMapper _mapper;

        public LocationService(ArcaiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedList<LocationDto>> GetAllLocationsAsync(PagingParameters pagingParameters)
        {
            var query = _context.Locations
                .AsNoTracking()
                .OrderBy(l => l.FriendlyName);

            var dtoQuery = query.ProjectTo<LocationDto>(_mapper.ConfigurationProvider);

            return await PagedList<LocationDto>.CreateAsync(
                dtoQuery,
                pagingParameters.PageNumber,
                pagingParameters.PageSize
            );
        }

        public async Task<LocationDto?> GetLocationByIdAsync(int id)
        {
            var entity = await _context.Locations.FindAsync(id);
            return entity == null ? null : _mapper.Map<LocationDto>(entity);
        }

        public async Task<LocationDto> AddLocationAsync(CreateLocationDto createDto)
        {
            var exists = await _context.Locations.AnyAsync(x => x.FriendlyName == createDto.FriendlyName);
            if (exists)
            {
                throw new InvalidOperationException("A Location with the same FriendlyName already exists.");
            }

            var entity = _mapper.Map<Location>(createDto);
            _context.Locations.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<LocationDto>(entity);
        }

        public async Task<LocationDto?> UpdateLocationAsync(int id, UpdateLocationDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return null;
            }

            var duplicate = await _context.Locations.AnyAsync(x => x.FriendlyName == updateDto.FriendlyName && x.Id != id);
            if (duplicate)
            {
                throw new InvalidOperationException("Another Location with the same FriendlyName already exists.");
            }

            var entity = await _context.Locations.FindAsync(id);
            if (entity == null)
            {
                return null;
            }

            _mapper.Map(updateDto, entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return _mapper.Map<LocationDto>(entity);
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            var entity = await _context.Locations.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _context.Locations.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
