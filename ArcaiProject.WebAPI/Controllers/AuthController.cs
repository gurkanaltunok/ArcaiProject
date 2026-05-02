using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArcaiProject.WebAPI.Controllers
{
    /// <summary>
    /// Kimlik doğrulama işlemleri için API controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        /// <summary>
        /// Kullanıcı giriş endpoint'i
        /// </summary>
        /// <param name="loginRequest">Email ve şifre bilgileri</param>
        /// <returns>Başarılı ise JWT token, başarısız ise Unauthorized</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var loginResponse = await _authService.LoginAsync(loginRequest);

            if (loginResponse == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(loginResponse);
        }

        /// <summary>
        /// Mevcut kullanıcı bilgisini döndürür
        /// </summary>
        /// <returns>Mevcut kullanıcı bilgisi</returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }
    }
}

