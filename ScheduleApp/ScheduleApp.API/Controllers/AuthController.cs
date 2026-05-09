using System;
// ScheduleApp.API/Controllers/AuthController.cs
namespace ScheduleApp.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Services;


/// Autor:  Mateo Quintero 
/// Version: 0.1

/// <summary>
/// Controlador de autenticación. Expone los endpoints relacionados con
/// el acceso al sistema: login, y en futuras ramas, logout y refresh token.
/// Ruta base: /api/auth
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService) => _authService = authService;

    /// <summary>
    /// Autentica al usuario con email y contraseña.
    /// Retorna un token JWT si las credenciales son válidas.
    /// </summary>
    /// <param name="request">Objeto con email y password.</param>
    /// <returns>
    /// 200 OK con token JWT y datos del usuario.
    /// 401 Unauthorized si las credenciales son incorrectas.
    /// 500 Internal Server Error si ocurre un error inesperado.
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor.", detail = ex.Message });
        }
    }
}