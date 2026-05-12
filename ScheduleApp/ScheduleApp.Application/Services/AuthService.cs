using System;

// ScheduleApp.Application/Services/AuthService.cs
using System.Security.Cryptography;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Services;

/// <summary>
/// Servicio de autenticación con persistencia de sesión mediante RefreshToken.
/// </summary>
/// Autor: Mateo Quintero
/// Version: 0.3
public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IPasswordHasher passwordHasher,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _refreshTokenRepository = refreshTokenRepository;
    }

    /// <summary>
    /// Login usando correo o username. Genera AccessToken + RefreshToken.
    /// </summary>
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository
            .GetByEmailOrUsernameAsync(request.Login);

        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Credenciales incorrectas.");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales incorrectas.");

        return await GenerateTokensAsync(user);
    }

    /// <summary>
    /// Renueva el AccessToken usando un RefreshToken válido.
    /// Implementa rotación de tokens por seguridad.
    /// </summary>
    public async Task<LoginResponseDto> RefreshAsync(string refreshToken)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (token is null || !token.IsActive)
            throw new UnauthorizedAccessException("Token de sesión inválido o expirado.");

        // Revocar el token usado (rotación)
        token.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(token);

        return await GenerateTokensAsync(token.User);
    }

    /// <summary>
    /// Cierra la sesión revocando todos los RefreshTokens del usuario.
    /// </summary>
    public async Task LogoutAsync(Guid userId)
    {
        await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);
    }

    /// <summary>
    /// Genera un nuevo par AccessToken + RefreshToken para el usuario.
    /// </summary>
    private async Task<LoginResponseDto> GenerateTokensAsync(User user)
    {
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshToken);

        return new LoginResponseDto
        {
            AccessToken = _jwtService.GenerateToken(user),
            RefreshToken = refreshToken.Token,
            ExpiresAt = _jwtService.GetExpiration(),
            UserName = user.FullName,
            Role = user.Role.Name
        };
    }
}