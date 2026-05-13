using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.Services;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.WebApi.Controllers;

[ApiController]
[Route("api/assignments")]
public class AssignmentController : ControllerBase
{
    private static readonly AvailabilityService service = new();

    [HttpPost]
    public IActionResult CreateAssignment([FromBody] Assignment assignment)
    {
        var result = service.SaveAssignment(assignment);

        if (result.Contains("already has") || result.Contains("occupied"))
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}