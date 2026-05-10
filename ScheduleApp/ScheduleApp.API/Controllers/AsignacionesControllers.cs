using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application;
using ScheduleApp.Application.Services;
using ScheduleApp.Domain.Entities;

[ApiController]
[Route("api/asignaciones")]
public class AsignacionesController : ControllerBase
{
    private static DisponibilidadService service = new DisponibilidadService();

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