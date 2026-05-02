using Microsoft.EntityFrameworkCore;
using ArcaiProject.DataAccess.Context;
using ArcaiProject.Entities.Settings;
using ArcaiProject.Business.Interfaces;
using ArcaiProject.Business.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using ArcaiProject.Entities.Entities;
using Microsoft.OpenApi.Models;
using AutoMapper;
using ArcaiProject.Business.MappingProfiles;
using ArcaiProject.WebAPI.BackgroundServices;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// --- DbContext Kayıt Alanı Başlangıcı ---
// 1. Connection string'i appsettings.json'dan oku
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. DbContext'i servislere ekle (Dependency Injection)
builder.Services.AddDbContext<ArcaiDbContext>(options =>
    options.UseSqlServer(connectionString));
// --- DbContext Kayıt Alanı Bitişi ---

// --- JWT Authentication Kayıt Alanı Başlangıcı ---

// 1. JwtSettings'i appsettings'den yükle
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// 2. Şifreleme (Password Hashing) servisini kaydet
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

// 3. AuthService'i DI container'a kaydet
builder.Services.AddScoped<IAuthService, AuthService>();

// Diğer servisler
builder.Services.AddScoped<IDocumentTypeService, DocumentTypeService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IAcademicPeriodService, AcademicPeriodService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IBorrowingRecordService, BorrowingRecordService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserService, UserService>();

// Background hosted service
builder.Services.AddHostedService<OverdueCheckService>();

// AutoMapper'ı kaydet (Assembly'den profilleri bulmasını sağla)
builder.Services.AddAutoMapper(typeof(DocumentTypeProfile).Assembly);

// 4. JWT Authentication'ı yapılandır
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Secret))
{
    throw new InvalidOperationException("JWT Secret key is not configured.");
}
var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Geliştirme ortamında false, produksiyonda true
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// 5. Authorization servisini ekle (daha sonra [Authorize] kullanabilmek için)
builder.Services.AddAuthorization();

// --- JWT Authentication Kayıt Alanı Bitişi ---

// --- CORS Yapılandırması Başlangıcı ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
// --- CORS Yapılandırması Bitişi ---

// Controllers'ı ekle
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// --- SWAGGER JWT YAPILANDIRMASI BAŞLANGICI ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Arcai API", Version = "v1" });

    // JWT için SecurityDefinition ekle
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [token] (e.g. 'Bearer your_token_here')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // JWT için SecurityRequirement ekle
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
// --- SWAGGER JWT YAPILANDIRMASI BİTİŞİ ---

var app = builder.Build();

// --- PASSWORD HASH UPDATE ON STARTUP (Sadece Development için) ---
// Veritabanındaki kullanıcı hash'lerini IPasswordHasher ile yeniden oluştur
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ArcaiDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        var adminUser = context.Users.FirstOrDefault(u => u.Email == "secretary@arcai.com");
        if (adminUser != null)
        {
            var tempAdmin = new User { Id = adminUser.Id };
            adminUser.PasswordHash = passwordHasher.HashPassword(tempAdmin, "AdminPassword123!");
            context.SaveChanges();
        }

        var profUser = context.Users.FirstOrDefault(u => u.Email == "ibrahim.ersan@arcai.com");
        if (profUser != null)
        {
            var tempProf = new User { Id = profUser.Id };
            profUser.PasswordHash = passwordHasher.HashPassword(tempProf, "ProfessorPassword123!");
            context.SaveChanges();
        }
    }
}
// --- PASSWORD HASH UPDATE BİTİŞİ ---

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS'u Authentication'dan ÖNCE ekle (ÖNEMLİ: Sıra önemli!)
app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

// ÖNEMLİ: Authentication ve Authorization'ı etkinleştir
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
