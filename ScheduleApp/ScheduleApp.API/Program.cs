// ScheduleApp.API/Program.cs

/// Autor:  Mateo Quintero 
/// Version: 0.1

// Punto de entrada de la aplicación. Configura todos los servicios,
// middleware, autenticación JWT e inyección de dependencias.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.Services;
using ScheduleApp.Infrastructure.Data;
using ScheduleApp.Infrastructure.Repositories;
using ScheduleApp.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// ── Base de datos ────────────────────────────────────────────────────────────
// Registra el AppDbContext con SQL Server usando la cadena de conexión
// definida en appsettings.Development.json → "DefaultConnection"
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Autenticación JWT ────────────────────────────────────────────────────────
// Configura el esquema Bearer para validar tokens JWT en cada request.
// Parámetros leídos desde appsettings.json sección "Jwt":
//   Secret   → clave para firmar/verificar el token (mínimo 32 caracteres)
//   Issuer   → quien emite el token (debe coincidir al validar)
//   Audience → destinatario del token (debe coincidir al validar)
//   ClockSkew = Zero → sin margen de tolerancia en la expiración
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

// ── Inyección de dependencias ────────────────────────────────────────────────
// Registra las implementaciones concretas para cada interfaz de la capa
// Application. Scope = una instancia por request HTTP.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>(); // ← aquí
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ── Migraciones automáticas ───────────────────────────────────────────────────
// Al iniciar la app aplica automáticamente las migraciones pendientes.
// Útil en desarrollo para no tener que correr dotnet ef database update manualmente.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Orden obligatorio: primero Authentication, luego Authorization
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
