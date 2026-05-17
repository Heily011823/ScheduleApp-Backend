using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.Services;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.WebApi.Controllers
{
    /*
     * Author: Salome Carmona
     * Feature: Classroom CRUD
     * Description: API endpoints for classroom management
     */
    [ApiController]
    [Route("api/[controller]")]
    public class ClassroomsController : ControllerBase
    {
        private readonly IClassroomService _classroomService;

        public ClassroomsController(IClassroomService classroomService)
        {
            _classroomService = classroomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var classrooms = await _classroomService.GetClassroomsAsync();

            return Ok(classrooms);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Classroom classroom)
        {
            await _classroomService.CreateClassroomAsync(classroom);

            return Ok(classroom);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Classroom classroom)
        {
            classroom.Id = id;

            await _classroomService.UpdateClassroomAsync(classroom);

            return Ok(classroom);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _classroomService.DeleteClassroomAsync(id);

            return Ok("Classroom deleted successfully");
        }
    }
}
