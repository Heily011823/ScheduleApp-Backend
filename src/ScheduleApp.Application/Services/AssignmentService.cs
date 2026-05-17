using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;

        public AssignmentService(IAssignmentRepository assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }

        public async Task SaveAssignmentAsync(Assignment assignment)
        {
            if (string.IsNullOrWhiteSpace(assignment.Teacher))
                throw new Exception("Teacher is required");

            if (string.IsNullOrWhiteSpace(assignment.Subject))
                throw new Exception("Subject is required");

            if (string.IsNullOrWhiteSpace(assignment.Classroom))
                throw new Exception("Classroom is required");

            if (assignment.StartTime >= assignment.EndTime)
                throw new Exception("Start time must be earlier than end time");

            /*
            * Author: Salome Carmona
            * Feature: Classroom Availability Validation
            * Description: Prevents duplicate classroom schedules
             */

            var classroomConflict = await _assignmentRepository
                .HasClassroomScheduleConflict(
                    assignment.Classroom,
                    assignment.Day,
                    assignment.StartTime,
                    assignment.EndTime);

            if (classroomConflict)
            {
                throw new Exception("The classroom is already occupied at this time");
            }
            await _assignmentRepository.CreateAsync(assignment);
            /*
             * Author: Salome Carmona
            * Feature: Teacher Schedule Validation
            * Description: Prevents overlapping teacher assignments
            */

            var hasConflict = await _assignmentRepository
                .HasTeacherScheduleConflict(
                    assignment.Teacher,
                    assignment.Day,
                    assignment.StartTime,
                    assignment.EndTime);

            if (hasConflict)
            {
                throw new Exception(
                    "The teacher already has an assigned class in this time range.");
            }
        }

        public async Task<List<Assignment>> GetAssignmentsAsync()
        {
            return await _assignmentRepository.GetAllAsync();
        }
    }
}