// ScheduleApp.API/Controllers/AsignacionesController.cs

/// Autor: Juan José Morales Aristizabal
/// Version: 0.1

// Controlador encargado de gestionar las asignaciones del sistema.
// Permite crear nuevas asignaciones validando disponibilidad
// de docentes y ocupación de aulas.

using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application;
using ScheduleApp.Application.Services;
using ScheduleApp.Domain.Entities;

[ApiController]
[Route("api/asignaciones")]
public class AsignacionesController : ControllerBase
{
    // ── Servicio de disponibilidad ─────────────────────────────────────────
    // Instancia encargada de gestionar la lógica de negocio relacionada
    // con la validación y almacenamiento de asignaciones.
    private static DisponibilidadService service = new DisponibilidadService();

    // ── Crear asignación ───────────────────────────────────────────────────
    // Endpoint HTTP POST encargado de registrar una nueva asignación.
    // Validaciones realizadas:
    //   - Verifica si el docente ya tiene una asignación.
    //   - Verifica si el aula ya se encuentra ocupada.
    // Respuestas:
    //   - 200 OK → asignación creada correctamente.
    //   - 400 BadRequest → conflicto en disponibilidad.
    [HttpPost]
    public IActionResult CrearAsignacion([FromBody] Asignacion asignacion)
    {
        var resultado = service.GuardarAsignacion(asignacion);

        if (resultado.Contains("ya tiene") || resultado.Contains("ocupada"))
        {
            return BadRequest(resultado);
        }

        return Ok(resultado);
    }
}