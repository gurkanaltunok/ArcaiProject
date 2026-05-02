using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Business.DTOs
{
    /// <summary>
    /// Kullanıcı giriş isteği için DTO
    /// </summary>
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}

