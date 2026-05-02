using System.ComponentModel.DataAnnotations;

namespace ArcaiProject.Business.DTOs
{
    public class CreateUserDto
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessage = "Password must have at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; } = string.Empty;

        // Admin'in hangi rolü atayacağını belirlemesi için (sadece Professor izin verilecek)
        [Required]
        [RegularExpression("^(Admin|Professor)$", ErrorMessage = "Role must be either 'Admin' or 'Professor'.")]
        public string Role { get; set; } = string.Empty;
    }
}

