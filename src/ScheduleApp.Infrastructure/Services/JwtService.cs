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
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    // El constructor ahora recibe las variables ya validadas desde Program.cs
    public JwtService(string jwtSecret, string jwtIssuer, string jwtAudience)
    {
        _jwtSecret = jwtSecret ?? throw new ArgumentNullException(nameof(jwtSecret));
        _jwtIssuer = jwtIssuer ?? throw new ArgumentNullException(nameof(jwtIssuer));
        _jwtAudience = jwtAudience ?? throw new ArgumentNullException(nameof(jwtAudience));
    }

    public string GenerateToken(User user)
    {
        // Eliminamos las lecturas de Environment.GetEnvironmentVariable y sus ifs redundantes

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSecret)); 

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,              
            audience: _jwtAudience,         
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