using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Helpers;
using ArcaiProject.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArcaiProject.WebAPI.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanıcılar erişebilir
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] PagingParameters pagingParameters)
        {
            var pagedUsers = await _userService.GetAllUsersAsync(pagingParameters);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedUsers.Metadata));

            return Ok(pagedUsers);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST: api/Users/create-professor
        // Admin, yeni Profesör hesapları oluşturabilir
        [HttpPost("create-professor")]
        public async Task<ActionResult<UserDto>> CreateProfessor([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Sadece Professor rolüyle kullanıcı oluşturmasına izin ver
            if (createUserDto.Role != "Professor")
            {
                return BadRequest(new { message = "Only 'Professor' role can be created via this endpoint." });
            }

            try
            {
                var createdUser = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); // Örn: "User with this email already exists."
            }
        }
        
        // PUT: api/Users/5/role
        // Admin, kullanıcının rolünü güncelleyebilir (kendi veya başka Admin rolünü değiştiremez)
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] string newRole)
        {
            if (string.IsNullOrEmpty(newRole))
            {
                return BadRequest(new { message = "Role cannot be empty." });
            }

            if (newRole != "Admin" && newRole != "Professor")
            {
                return BadRequest(new { message = "Role must be either 'Admin' or 'Professor'." });
            }

            if (!await _userService.UserExistsAsync(id))
            {
                return NotFound();
            }

            try
            {
                var result = await _userService.UpdateUserRoleAsync(id, newRole);
                if (!result)
                {
                    return BadRequest(new { message = "Failed to update user role." });
                }
                return NoContent(); // Başarılı, içerik döndürme
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "User not found or already deleted." });
                }
                return NoContent(); // Başarılı, içerik döndürme
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                return StatusCode(500, new { message = "An error occurred while deleting the user.", details = ex.Message });
            }
        }
    }
}

