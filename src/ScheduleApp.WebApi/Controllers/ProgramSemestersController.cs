using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.Services;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/programs/{programId}/semesters")]
    public class ProgramSemestersController : ControllerBase
    {
        private readonly ProgramSemesterService _service;

        public ProgramSemestersController(ProgramSemesterService service)
        {
            _service = service;
        }

        /*
         * Author: Salome Carmona
         * Feature: CRUD Program Semesters
         * Description: Gets semesters by program
         */
        [HttpGet]
        public async Task<IActionResult> Get(Guid programId)
        {
            var semesters = await _service.GetByProgramIdAsync(programId);

            return Ok(semesters);
        }

        /*
         * Author: Salome Carmona
         * Feature: CRUD Program Semesters
         * Description: Updates semester credits
         */
        [HttpPut]
        public async Task<IActionResult> Update(
            Guid programId,
            [FromBody] List<ProgramSemester> semesters)
        {
            await _service.UpdateAsync(semesters);

            return NoContent();
        }
    }
}