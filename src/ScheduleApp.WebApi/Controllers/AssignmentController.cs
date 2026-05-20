using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/assignments")]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpPost]
        public async Task<IActionResult> SaveAssignment([FromBody] Assignment assignment)
        {
            try
            {
                await _assignmentService.SaveAssignmentAsync(assignment);
                return Ok(new { message = "Assignment saved successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAssignments()
        {
            var assignments = await _assignmentService.GetAssignmentsAsync();
            return Ok(assignments);
        }

        // HU-73: detalle de Assignment por Id
        // Autor: Jacobo
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAssignmentById(int id)
        {
            var assignment = await _assignmentService.GetAssignmentByIdAsync(id);

            if (assignment is null)
                return NotFound(new { message = $"No se encontro la asignacion con Id '{id}'." });

            return Ok(assignment);
        }

        // HU-73: actualizar Assignment existente
        // Autor: Jacobo
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAssignment(int id, [FromBody] Assignment assignment)
        {
            try
            {
                var updated = await _assignmentService.UpdateAssignmentAsync(id, assignment);

                if (updated is null)
                    return NotFound(new { message = $"No se encontro la asignacion con Id '{id}'." });

                return Ok(new
                {
                    message = "Assignment updated successfully",
                    assignment = updated
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // HU-73: eliminar Assignment por Id
        // Autor: Jacobo
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var deleted = await _assignmentService.DeleteAssignmentAsync(id);

            if (!deleted)
                return NotFound(new { message = $"No se encontro la asignacion con Id '{id}'." });

            return Ok(new { message = "Assignment deleted successfully" });
        }
    }
}
