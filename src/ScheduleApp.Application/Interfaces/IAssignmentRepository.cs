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

        /*
      * Author: Salome Carmona
       * Feature: Classroom Availability Validation
       * Description: Validates classroom schedule conflicts
       */
        Task<bool> HasClassroomScheduleConflict(
        string classroom,
        int day,
        TimeSpan startTime,
        TimeSpan endTime);
    }
}