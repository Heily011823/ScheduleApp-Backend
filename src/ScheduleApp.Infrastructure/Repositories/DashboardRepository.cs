using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _context;

        public DashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public DashboardSummaryDto GetSummary()
        {
            return new DashboardSummaryDto
            {
                Subjects = _context.Subjects.Count(),

                Teachers = _context.Teachers.Count(),

                Schedules = _context.Schedules.Count(),

                Programs = _context.AcademicPrograms
                    .Count(p => !p.IsDeleted),

                Classrooms = _context.Classrooms.Count(),

                Coordinators = _context.Users
                    .Include(u => u.Role)
                    .Count(u =>
                        u.Role != null &&
                        u.Role.Name == "Coordinador"
                    )
            };
        }
    }
}