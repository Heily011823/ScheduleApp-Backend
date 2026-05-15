using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.DTOs;

namespace ScheduleApp.WebApi.Controllers;

[ApiController]
[Route("api/subjects")]
public class SubjectController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSubject(Guid id)
    {
        try
        {
            await _subjectService.DeleteSubjectAsync(id);
            return Ok("Subject deleted successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubject(CreateSubjectDto dto)
    {
        try
        {
            await _subjectService.CreateSubjectAsync(dto);
            return Ok("Subject created successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSubject(Guid id, UpdateSubjectDto dto)
    {
        try
        {
            await _subjectService.UpdateSubjectAsync(id, dto);
            return Ok("Subject updated successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}