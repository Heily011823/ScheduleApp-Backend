using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.Interfaces;

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
}