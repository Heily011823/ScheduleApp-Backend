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

        [HttpPost]
        public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentDto dto)
        {
            try
            {
                var created = await _assignmentService.CreateAssignmentAsync(dto);
                return CreatedAtAction(
                    nameof(GetAssignmentById),
                    new { id = created.Id },
                    created);
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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAssignmentById(int id)
        {
            var assignment = await _assignmentService.GetAssignmentByIdAsync(id);

            if (assignment is null)
            {
                return NotFound(new
                {
                    message = $"No se encontro un horario con el Id '{id}'."
                });
            }

            return Ok(assignment);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAssignment(
            int id,
            [FromBody] UpdateAssignmentDto dto)
        {
            try
            {
                var updated = await _assignmentService.UpdateAssignmentAsync(id, dto);

                if (updated is null)
                {
                    return NotFound(new
                    {
                        message = $"No se encontro un horario con el Id '{id}'."
                    });
                }

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var deleted = await _assignmentService.DeleteAssignmentAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"No se encontro un horario con el Id '{id}'."
                });
            }

            return NoContent();
        }
    }
}
