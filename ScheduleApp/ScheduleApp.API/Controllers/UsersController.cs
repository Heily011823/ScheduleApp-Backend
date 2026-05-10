using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(
            [FromQuery] string? name,
            [FromQuery] string? role,
            [FromQuery] bool? isActive)
        {
            try
            {
                var users = await _userService.SearchUsersAsync(
                    name,
                    role,
                    isActive);

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}