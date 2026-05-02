using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Helpers;

namespace ArcaiProject.Business.Interfaces
{
    public interface IUserService
    {
        Task<PagedList<UserDto>> GetAllUsersAsync(PagingParameters pagingParameters);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        // Admin sadece Professor veya kendini güncelleyebilir, şifre değişikliği ayrı bir metot olabilir.
        // Bu yüzden şimdilik Update'i UserDto ile yapmayıp sadece rolleri güncelleyelim.
        Task<bool> UpdateUserRoleAsync(int id, string newRole);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(int id);
    }
}

