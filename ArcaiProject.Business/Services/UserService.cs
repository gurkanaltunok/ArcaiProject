using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Helpers;
using ArcaiProject.Business.Interfaces;
using ArcaiProject.DataAccess.Context;
using ArcaiProject.Entities.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArcaiProject.Business.Services
{
    public class UserService : IUserService
    {
        private readonly ArcaiDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(ArcaiDbContext context, IMapper mapper, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<PagedList<UserDto>> GetAllUsersAsync(PagingParameters pagingParameters)
        {
            var query = _context.Users
                .AsNoTracking()
                .Where(u => u.IsActive == true) // Only get active users
                .OrderBy(u => u.LastName); // Soyada göre sıralama

            var dtoQuery = query.ProjectTo<UserDto>(_mapper.ConfigurationProvider);

            return await PagedList<UserDto>.CreateAsync(dtoQuery, pagingParameters.PageNumber, pagingParameters.PageSize);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive == true);
            
            if (user == null)
            {
                return null;
            }
            
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // E-posta zaten kullanılıyor mu kontrolü (only check active users)
            if (await _context.Users.AnyAsync(u => u.Email == createUserDto.Email && u.IsActive == true))
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            // Sadece Professor rolüyle yeni kullanıcı oluşturulmasına izin verelim (Admin kendi kendini oluşturamaz)
            if (createUserDto.Role != "Professor")
            {
                throw new InvalidOperationException("Only 'Professor' role can be created via this method.");
            }

            var user = _mapper.Map<User>(createUserDto);
            user.PasswordHash = _passwordHasher.HashPassword(user, createUserDto.Password); // Şifreyi hash'le
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true; // New users are active by default

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> UpdateUserRoleAsync(int id, string newRole)
        {
            if (newRole != "Admin" && newRole != "Professor")
            {
                throw new InvalidOperationException("Role must be either 'Admin' or 'Professor'.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive == true);
            if (user == null)
            {
                return false;
            }

            // Admin'in kendi rolünü API üzerinden değiştirmesini veya başka bir Admin'in rolünü değiştirmesini engelle
            // Bu basit bir kural, gerçekte daha kompleks yetki kontrolleri olabilir
            if (user.Role == "Admin" && newRole != "Admin")
            {
                throw new InvalidOperationException("Cannot change the role of an existing Admin user via this method.");
            }
            if (newRole == "Admin")
            {
                throw new InvalidOperationException("Cannot set user role to Admin via this method. Only existing admin can manage roles.");
            }

            user.Role = newRole;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            // Use FirstOrDefaultAsync with tracking to update the entity
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
            
            if (user == null)
            {
                return false;
            }
            
            if (!user.IsActive)
            {
                // User is already deleted
                return false;
            }
            
            // Admin'in kendi hesabını silmesini engelle (veya son admini)
            if (user.Role == "Admin")
            {
                throw new InvalidOperationException("Cannot delete an Admin user via this method.");
            }

            // Soft delete: Set IsActive to false instead of removing
            user.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id && u.IsActive == true);
        }
    }
}

