using System;

// ScheduleApp.Application/Services/AuthService.cs
namespace ScheduleApp.Application.Services;

using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

/// <summary>
/// Servicio de aplicación que gestiona la lógica de autenticación.
/// Orquesta la validación de credenciales y la generación de tokens JWT.
/// </summary>
/// Autor:  Mateo Quintero 
/// Version: 0.1


public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Constructor con inyección de dependencias.
    /// </summary>
    /// <param name="userRepository">Repositorio para consultar usuarios en BD.</param>
    /// <param name="jwtService">Servicio para generar tokens JWT.</param>
    /// <param name="passwordHasher">Servicio para verificar contraseñas con BCrypt.</param>
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
    /// Autentica a un usuario validando sus credenciales contra la base de datos.
    /// </summary>
    /// <param name="request">DTO con email y contraseña del usuario.</param>
    /// <returns>DTO con el token JWT y datos del usuario autenticado.</returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Se lanza cuando el usuario no existe, está inactivo o la contraseña es incorrecta.
    /// </exception>
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
