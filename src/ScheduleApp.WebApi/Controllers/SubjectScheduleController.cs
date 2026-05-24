using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;
namespace ScheduleApp.WebApi.Controllers;

[ApiController]
[Route("api/subject-schedules")]
[Authorize]
public class SubjectScheduleController : ControllerBase
{
    private readonly AppDbContext _context;

    public SubjectScheduleController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubjectScheduleItemDto dto)
    {
        try
        {
            var schedule = new SubjectSchedule
            {
                Id = Guid.NewGuid(),
                SubjectId = dto.SubjectId,
                DayOfWeek = dto.Day,
                StartHour = dto.StartTime,
                EndHour = dto.EndTime,
                CreatedAt = DateTime.UtcNow
            };

            await _context.SubjectSchedules.AddAsync(schedule);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Horario de materia guardado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{subjectId:guid}")]
    public async Task<IActionResult> GetBySubject(Guid subjectId)
    {
        var schedules = await _context.SubjectSchedules
            .Where(s => s.SubjectId == subjectId)
            .Select(s => new
            {
                s.Id,
                s.SubjectId,
                s.DayOfWeek,
                s.StartHour,
                s.EndHour
            })
            .ToListAsync();

        return Ok(schedules);
    }
}