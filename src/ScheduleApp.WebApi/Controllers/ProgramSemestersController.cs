using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Domain.Entities;
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
            var program = await _context.AcademicPrograms
                .FirstOrDefaultAsync(p => p.Id == programId && !p.IsDeleted);

            if (program == null)
            {
                return NotFound(new
                {
                    message = $"No existe el programa con Id '{programId}'."
                });
            }

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

            var program = await _context.AcademicPrograms
                .FirstOrDefaultAsync(p => p.Id == programId && !p.IsDeleted);

            if (program == null)
            {
                return NotFound(new
                {
                    message = $"No existe el programa con Id '{programId}'."
                });
            }

            if (semesters.Count != program.TotalSemesters)
            {
                return BadRequest(new
                {
                    message = $"Debe configurar exactamente {program.TotalSemesters} semestre(s) para este programa."
                });
            }

            foreach (var item in semesters)
            {
                if (item.Semester <= 0)
                {
                    return BadRequest(new
                    {
                        message = $"El número de semestre '{item.Semester}' no es válido."
                    });
                }

                if (item.Semester > program.TotalSemesters)
                {
                    return BadRequest(new
                    {
                        message = $"El semestre {item.Semester} supera el total de semestres del programa."
                    });
                }

                if (item.MaxCredits <= 6)
                {
                    return BadRequest(new
                    {
                        message = $"Los créditos del semestre {item.Semester} deben ser mayores a 6."
                    });
                }
            }

            var duplicatedSemester = semesters
                .GroupBy(s => s.Semester)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicatedSemester != null)
            {
                return BadRequest(new
                {
                    message = $"El semestre {duplicatedSemester.Key} está repetido."
                });
            }

            var expectedSemesters = Enumerable.Range(1, program.TotalSemesters).ToList();

            var missingSemester = expectedSemesters
                .FirstOrDefault(number => !semesters.Any(s => s.Semester == number));

            if (missingSemester > 0)
            {
                return BadRequest(new
                {
                    message = $"Falta configurar el semestre {missingSemester}."
                });
            }

            var existingSemesters = await _context.ProgramSemesters
                .Where(s => s.AcademicProgramId == programId)
                .ToListAsync();

            foreach (var item in semesters)
            {
                var semester = existingSemesters
                    .FirstOrDefault(s => s.SemesterNumber == item.Semester);

                if (semester == null)
                {
                    semester = new ProgramSemester
                    {
                        Id = Guid.NewGuid(),
                        AcademicProgramId = programId,
                        SemesterNumber = item.Semester,
                        MaxCredits = item.MaxCredits,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.ProgramSemesters.Add(semester);
                }
                else
                {
                    semester.MaxCredits = item.MaxCredits;
                    semester.UpdatedAt = DateTime.UtcNow;
                }
            }

            var semestersToRemove = existingSemesters
                .Where(existing => !semesters.Any(s => s.Semester == existing.SemesterNumber))
                .ToList();

            if (semestersToRemove.Count > 0)
            {
                _context.ProgramSemesters.RemoveRange(semestersToRemove);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Créditos actualizados correctamente.",
                programId,
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