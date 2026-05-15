using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly int _expirationMinutes = 60;

    public string GenerateToken(User user)
    {
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
        var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
        var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

        if (string.IsNullOrWhiteSpace(jwtSecret))
            throw new InvalidOperationException("Falta la variable JWT_SECRET en el archivo .env");

        if (string.IsNullOrWhiteSpace(jwtIssuer))
            throw new InvalidOperationException("Falta la variable JWT_ISSUER en el archivo .env");

        if (string.IsNullOrWhiteSpace(jwtAudience))
            throw new InvalidOperationException("Falta la variable JWT_AUDIENCE en el archivo .env");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSecret));

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: GetExpiration(),
            signingCredentials: new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetExpiration()
    {
        return DateTime.UtcNow.AddMinutes(_expirationMinutes);
    }
}