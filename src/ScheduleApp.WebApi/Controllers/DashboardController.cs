using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.Services;

namespace ScheduleApp.WebApi.Controllers
{
    /// <summary>
    /// Controller responsible for dashboard endpoints.
    /// </summary>
    /// <author>Salome Carmona</author>
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        /// <summary>
        /// Dashboard controller constructor.
        /// </summary>
        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Returns dashboard summary information.
        /// </summary>
        /// <returns>Dashboard totals.</returns>
        [HttpGet("summary")]
        public IActionResult GetSummary()
        {
            try
            {
                var summary = _dashboardService.GetSummary();

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error retrieving dashboard summary",
                    error = ex.Message
                });
            }
        }
    }
}