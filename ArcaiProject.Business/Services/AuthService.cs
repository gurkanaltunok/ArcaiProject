using ArcaiProject.Business.DTOs;
using ArcaiProject.Business.Interfaces;
using ArcaiProject.DataAccess.Context;
using ArcaiProject.Entities.Settings;
using ArcaiProject.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;

namespace ArcaiProject.Business.Services
{
    /// <summary>
    /// Kimlik doğrulama servisi implementasyonu
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ArcaiDbContext _context;
        private readonly JwtSettings _jwtSettings;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(ArcaiDbContext context, IOptions<JwtSettings> jwtSettings, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            // 1. Kullanıcıyı e-postaya göre bul (only active users)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginRequest.Email && u.IsActive == true);

            if (user == null)
            {
                // Kullanıcı bulunamadı veya aktif değil
                return null;
            }

            // 2. Şifreyi doğrula
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                // Şifre yanlış
                return null;
            }

            // 3. Şifre doğru, JWT Token oluştur
            var token = GenerateJwtToken(user);

            // 4. UserDto'yu oluştur
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            };

            return new LoginResponseDto 
            { 
                Token = token,
                User = userDto
            };
        }

        /// <summary>
        /// Kullanıcı bilgilerinden JWT token oluşturur
        /// </summary>
        /// <param name="user">Kullanıcı entity'si</param>
        /// <returns>JWT token string</returns>
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            // Token'ın içine (payload) hangi verileri koyacağımızı belirliyoruz
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

