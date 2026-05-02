using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Helpers;

namespace ArcaiProject.Business.Interfaces
{
    public interface ICourseService
    {
        Task<PagedList<CourseDto>> GetAllCoursesAsync(PagingParameters pagingParameters);
        Task<CourseDto?> GetCourseByIdAsync(int id);
        Task<CourseDto> AddCourseAsync(CreateCourseDto createDto);
        Task<CourseDto?> UpdateCourseAsync(int id, UpdateCourseDto updateDto);
        Task<bool> DeleteCourseAsync(int id);
    }
}
