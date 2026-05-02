using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Helpers;

namespace ArcaiProject.Business.Interfaces
{
    public interface IAcademicPeriodService
    {
        Task<PagedList<AcademicPeriodDto>> GetAllAcademicPeriodsAsync(PagingParameters pagingParameters);
        Task<AcademicPeriodDto?> GetAcademicPeriodByIdAsync(int id);
        Task<AcademicPeriodDto> AddAcademicPeriodAsync(CreateAcademicPeriodDto createDto);
        Task<AcademicPeriodDto?> UpdateAcademicPeriodAsync(int id, UpdateAcademicPeriodDto updateDto);
        Task<bool> DeleteAcademicPeriodAsync(int id);
    }
}
