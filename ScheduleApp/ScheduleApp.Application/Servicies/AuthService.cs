using System;

// ScheduleApp.Application/Services/AuthService.cs
namespace ScheduleApp.Application.Services;

using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

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

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Credenciales incorrectas.");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales incorrectas.");

        return new LoginResponseDto
        {
            AccessToken = _jwtService.GenerateToken(user),
            ExpiresAt = _jwtService.GetExpiration(),
            UserName = user.FullName,
            Role = user.Role
        };
    }
}
