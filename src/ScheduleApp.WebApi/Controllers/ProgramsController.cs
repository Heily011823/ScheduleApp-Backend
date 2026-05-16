using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

namespace ScheduleApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/programs")]
    public class ProgramsController : ControllerBase
    {
        private readonly IProgramService _service;

        public ProgramsController(IProgramService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var programs = await _service.GetAllAsync();
            return Ok(programs);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var program = await _service.GetByIdAsync(id);

            if (program is null)
            {
                return NotFound(new
                {
                    message = $"No se encontro un programa con el Id '{id}'."
                });
            }

            return Ok(program);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProgramDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProgramDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);

                if (updated is null)
                {
                    return NotFound(new
                    {
                        message = $"No se encontro un programa con el Id '{id}'."
                    });
                }

                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"No se encontro un programa con el Id '{id}'."
                });
            }

            return NoContent();
        }
    }
}
