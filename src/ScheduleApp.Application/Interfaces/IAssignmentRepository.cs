using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces
{
    public interface IAssignmentRepository
    {
        Task CreateAsync(Assignment assignment);

        Task<List<Assignment>> GetAllAsync();

        Task<bool> HasTeacherScheduleConflict(
            string teacher,
            int day,
            TimeSpan startTime,
            TimeSpan endTime);
    }
}