using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Helpers;

namespace ArcaiProject.Business.Interfaces
{
    public interface ITagService
    {
        Task<PagedList<TagDto>> GetAllTagsAsync(PagingParameters pagingParameters);
        Task<TagDto?> GetTagByIdAsync(int id);
        Task<TagDto> AddTagAsync(CreateTagDto createDto);
        Task<TagDto?> UpdateTagAsync(int id, UpdateTagDto updateDto);
        Task<bool> DeleteTagAsync(int id);
    }
}
