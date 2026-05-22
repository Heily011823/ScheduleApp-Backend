using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Services;

// ScheduleApp.API/Controllers/AuthController.cs
namespace ScheduleApp.API.Controllers;

/// Autor: Mateo Quintero 
/// Version: 0.1

/// <summary>
/// Controlador de autenticación. Expone los endpoints relacionados con
/// el acceso al sistema: login, logout y refresh token.
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

    /// <summary>
    /// Cierra la sesión del usuario autenticado.
    /// En JWT el cierre de sesión consiste en invalidar/eliminar
    /// el token del lado del cliente.
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        try
        {
            return Ok(new
            {
                success = true,
                message = "Sesión cerrada correctamente."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Error al cerrar sesión.",
                detail = ex.Message
            });
        }
    }
}