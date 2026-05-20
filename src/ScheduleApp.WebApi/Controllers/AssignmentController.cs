using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

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

        // Crear un nuevo Assignment
        [HttpPost]
        public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentDto dto)
        {
            try
            {
                var created = await _assignmentService.CreateAssignmentAsync(dto);

                return CreatedAtAction(
                    nameof(GetAssignmentById),
                    new { id = created.Id },
                    created
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Obtener todos los Assignments
        [HttpGet]
        public async Task<IActionResult> GetAssignments()
        {
            var assignments = await _assignmentService.GetAssignmentsAsync();
            return Ok(assignments);
        }

        // Obtener Assignment por Id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAssignmentById(int id)
        {
            var assignment = await _assignmentService.GetAssignmentByIdAsync(id);

            if (assignment is null)
                return NotFound(new { message = $"No se encontro la asignacion con Id '{id}'." });

            return Ok(assignment);
        }

        // Actualizar Assignment existente
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAssignment(
            int id,
            [FromBody] UpdateAssignmentDto dto)
        {
            try
            {
                var updated = await _assignmentService.UpdateAssignmentAsync(id, dto);

                if (updated is null)
                    return NotFound(new { message = $"No se encontro la asignacion con Id '{id}'." });

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Eliminar Assignment por Id
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