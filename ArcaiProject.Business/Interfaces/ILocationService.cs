using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Helpers;

namespace ArcaiProject.Business.Interfaces
{
    public interface ILocationService
    {
        Task<PagedList<LocationDto>> GetAllLocationsAsync(PagingParameters pagingParameters);
        Task<LocationDto?> GetLocationByIdAsync(int id);
        Task<LocationDto> AddLocationAsync(CreateLocationDto createDto);
        Task<LocationDto?> UpdateLocationAsync(int id, UpdateLocationDto updateDto);
        Task<bool> DeleteLocationAsync(int id);
    }
}
