namespace ArcaiProject.Business.DTOs
{
    /// <summary>
    /// Başarılı giriş sonrası dönen JWT token ve kullanıcı bilgileri
    /// </summary>
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
    }
}

