using System;

// ScheduleApp.API/Controllers/AuthController.cs
namespace ScheduleApp.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Services;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService) => _authService = authService;

    /// <summary>Autenticar usuario con email y contraseña</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);                          // 200 - credenciales correctas
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message }); // 401 - credenciales incorrectas
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error interno del servidor." }); // 500
        }
    }
}
