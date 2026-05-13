using System;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

namespace ScheduleApp.Application.Services;

/// <summary>
/// Servicio de autenticación.
/// Permite login con correo institucional o username.
/// </summary>
/// Autor: Mateo Quintero
/// Version: 0.2
public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Login usando correo o username.
    /// </summary>
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository
            .GetByEmailOrUsernameAsync(request.Login);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "Credenciales incorrectas.");
        }

        var passwordValid = _passwordHasher.Verify(
            request.Password,
            user.PasswordHash);

        if (!passwordValid)
        {
            throw new UnauthorizedAccessException(
                "Credenciales incorrectas.");
        }

        return new LoginResponseDto
        {
            AccessToken = _jwtService.GenerateToken(user),
            ExpiresAt = _jwtService.GetExpiration(),
            UserName = user.FullName,
            Role = user.Role.Name
        };
    }
}