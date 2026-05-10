using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.Services;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.API.Controllers
{
    [ApiController]
    [Route("api/classroom")]
    public class ClassroomController : ControllerBase
    {
        private static ClassroomAvailabilityService service = new ClassroomAvailabilityService();

        [HttpPost("assignment")]
        public IActionResult CreateAssignment(
            [FromBody]
            ClassroomAssignment assignment)
        {
            var result =
                service.SaveAssignment(
                    assignment);

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
                message = result
            });
        }
    }
}
