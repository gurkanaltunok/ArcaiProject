using ArcaiProject.Business.DTOs;

namespace ArcaiProject.Business.Interfaces
{
    /// <summary>
    /// Kimlik doğrulama servisi interface'i
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Kullanıcı girişi yapar ve JWT token döner
        /// </summary>
        /// <param name="loginRequest">Giriş bilgileri (email ve password)</param>
        /// <returns>Başarılı ise JWT token, başarısız ise null</returns>
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);
    }
}

