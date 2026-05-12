using System;


// ScheduleApp.API/Controllers/AuthController.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Services;

namespace ScheduleApp.API.Controllers;

/// <summary>
/// Controlador de autenticación y persistencia de sesión.
/// Ruta base: /api/auth
/// </summary>
/// Autor: Mateo Quintero
/// Version: 0.3
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService) => _authService = authService;

    /// <summary>
    /// Autentica al usuario. Retorna AccessToken + RefreshToken.
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
    /// Renueva el AccessToken usando un RefreshToken válido.
    /// Permite mantener la sesión activa sin re-autenticar.
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var response = await _authService.RefreshAsync(request.RefreshToken);
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
    /// Cierra la sesión revocando todos los RefreshTokens del usuario.
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
                return Unauthorized(new { message = "Usuario no identificado." });

            var userId = Guid.Parse(userIdClaim);
            await _authService.LogoutAsync(userId);

            return Ok(new { success = true, message = "Sesión cerrada correctamente." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Error al cerrar sesión.", detail = ex.Message });
        }
    }
}