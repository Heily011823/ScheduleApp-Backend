using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/programs/{programId:guid}/semesters")]
    public class ProgramSemestersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProgramSemestersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid programId)
        {
            var semesters = await _context.ProgramSemesters
                .Where(s => s.AcademicProgramId == programId)
                .OrderBy(s => s.SemesterNumber)
                .Select(s => new ProgramSemesterDto
                {
                    Id = s.Id,
                    ProgramId = s.AcademicProgramId,
                    Semester = s.SemesterNumber,
                    MaxCredits = s.MaxCredits
                })
                .ToListAsync();

            return Ok(semesters);
        }

        [HttpPut]
        public async Task<IActionResult> Update(
            Guid programId,
            [FromBody] List<ProgramSemesterDto> semesters)
        {
            if (semesters == null || semesters.Count == 0)
            {
                return BadRequest(new
                {
                    message = "Debe enviar al menos un semestre."
                });
            }

            var programExists = await _context.AcademicPrograms
                .AnyAsync(p => p.Id == programId && !p.IsDeleted);

            if (!programExists)
            {
                return NotFound(new
                {
                    message = $"No existe el programa con Id '{programId}'."
                });
            }

            var existingSemesters = await _context.ProgramSemesters
                .Where(s => s.AcademicProgramId == programId)
                .ToListAsync();

            foreach (var item in semesters)
            {
                if (item.Semester <= 0)
                {
                    return BadRequest(new
                    {
                        message = $"El número de semestre '{item.Semester}' no es válido."
                    });
                }

                if (item.MaxCredits <= 0)
                {
                    return BadRequest(new
                    {
                        message = $"Los créditos del semestre {item.Semester} deben ser mayores a 0."
                    });
                }

                var semester = existingSemesters
                    .FirstOrDefault(s => s.SemesterNumber == item.Semester);

                if (semester == null)
                {
                    return BadRequest(new
                    {
                        message = $"El semestre {item.Semester} no existe para este programa."
                    });
                }

                semester.MaxCredits = item.MaxCredits;
                semester.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Créditos actualizados correctamente.",
                programId = programId,
                totalSemestersUpdated = semesters.Count
            });
        }
    }

    public class ProgramSemesterDto
    {
        public Guid Id { get; set; }

        public Guid ProgramId { get; set; }

        public int Semester { get; set; }

        public int MaxCredits { get; set; }
    }
}