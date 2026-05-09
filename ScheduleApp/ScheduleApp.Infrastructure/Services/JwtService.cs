using System;

// ScheduleApp.Infrastructure/Services/JwtService.cs
namespace ScheduleApp.Infrastructure.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

/// <summary>
/// Servicio de infraestructura para la generación de tokens JWT.
/// Lee la configuración desde appsettings.json (sección "Jwt").
/// </summary>
/// 
/// Autor:  Mateo Quintero 
/// Version: 0.1
/// 
public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    /// <summary>Duración del token en minutos. Por defecto 60 minutos.</summary>
    private readonly int _expirationMinutes = 60;

    public JwtService(IConfiguration config) => _config = config;

    /// <summary>
    /// Genera un token JWT firmado con los claims del usuario.
    /// Incluye: Id, Email, Nombre y Rol.
    /// </summary>
    /// <param name="user">Usuario autenticado del cual se extraen los claims.</param>
    /// <returns>Token JWT como string.</returns>
    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: GetExpiration(),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Calcula la fecha y hora de expiración del token desde el momento actual.
    /// </summary>
    /// <returns>DateTime en UTC con la expiración del token.</returns>
    public DateTime GetExpiration() =>
        DateTime.UtcNow.AddMinutes(_expirationMinutes);
}