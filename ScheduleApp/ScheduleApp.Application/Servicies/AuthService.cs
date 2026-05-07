using System;

// ScheduleApp.Application/Services/AuthService.cs
namespace ScheduleApp.Application.Services;

using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Criterio: validar que el usuario existe en BD
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Credenciales incorrectas.");

        // Criterio: validar contraseña
        bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!passwordValid)
            throw new UnauthorizedAccessException("Credenciales incorrectas.");

        // Criterio: credenciales correctas → generar token
        return new LoginResponseDto
        {
            AccessToken = _jwtService.GenerateToken(user),
            ExpiresAt = _jwtService.GetExpiration(),
            UserName = user.FullName,
            Role = user.Role
        };
    }
}
