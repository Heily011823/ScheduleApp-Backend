using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces
{
    public interface IAssignmentRepository
    {
        Task CreateAsync(Assignment assignment);

        Task<List<Assignment>> GetAllAsync();

        Task<Assignment?> GetByIdAsync(int id);

        Task UpdateAsync(Assignment assignment);

        Task<bool> DeleteAsync(int id);

        Task<bool> HasTeacherScheduleConflict(
            string teacher,
            int day,
            TimeSpan startTime,
            TimeSpan endTime
        );

        Task<bool> HasTeacherScheduleConflictExcluding(
            string teacher,
            int day,
            TimeSpan startTime,
            TimeSpan endTime,
            int excludeAssignmentId
        );

        Task<bool> HasClassroomScheduleConflict(
            string classroom,
            int day,
            TimeSpan startTime,
            TimeSpan endTime
        );

        Task<bool> HasClassroomScheduleConflictExcluding(
            string classroom,
            int day,
            TimeSpan startTime,
            TimeSpan endTime,
            int excludeAssignmentId
        );
    }
}