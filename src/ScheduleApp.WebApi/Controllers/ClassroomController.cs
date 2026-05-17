using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.Services;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.API.Controllers;

[ApiController]
[Route("api/classroom")]
public class ClassroomController : ControllerBase
{
    private readonly IClassroomAvailabilityService service;

    public ClassroomController(IClassroomAvailabilityService service)
    {
        this.service = service;
    }

    [HttpPost("assignment")]
    public IActionResult CheckAvailability([FromBody] ClassroomAssignment assignment)
    {
        if (assignment == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "La asignación es obligatoria."
            });
        }

        if (assignment.ClassroomId == Guid.Empty)
        {
            return BadRequest(new
            {
                success = false,
                message = "Debe seleccionar un aula válida."
            });
        }

        if (assignment.Date == default)
        {
            return BadRequest(new
            {
                success = false,
                message = "La fecha es obligatoria."
            });
        }

        if (assignment.StartTime == default || assignment.EndTime == default)
        {
            return BadRequest(new
            {
                success = false,
                message = "El horario es obligatorio."
            });
        }

        if (assignment.StartTime >= assignment.EndTime)
        {
            return BadRequest(new
            {
                success = false,
                message = "La hora de inicio debe ser menor que la hora de fin."
            });
        }

        var result = service.SaveAssignment(assignment);

        if (result.Contains("ocupada"))
        {
            return BadRequest(new
            {
                success = false,
                message = result
            });
        }

        return Ok(new
        {
            success = true,
            message = result,
            dayOfWeek = assignment.Date.DayOfWeek.ToString()
        });
    }
}