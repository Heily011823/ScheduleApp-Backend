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

            var classroomConflict = await _assignmentRepository
                .HasClassroomScheduleConflict(
                    assignment.Classroom,
                    assignment.Day,
                    assignment.StartTime,
                    assignment.EndTime);
            if (classroomConflict)
                throw new Exception("The classroom is already occupied at this time");

            await _assignmentRepository.CreateAsync(assignment);

            var hasConflict = await _assignmentRepository
                .HasTeacherScheduleConflict(
                    assignment.Teacher,
                    assignment.Day,
                    assignment.StartTime,
                    assignment.EndTime);
            if (hasConflict)
                throw new Exception(
                    "The teacher already has an assigned class in this time range.");
        }

        public async Task<List<Assignment>> GetAssignmentsAsync()
        {
            return await _assignmentRepository.GetAllAsync();
        }

        public async Task<Assignment?> GetAssignmentByIdAsync(int id)
        {
            return await _assignmentRepository.GetByIdAsync(id);
        }

        public async Task<Assignment?> UpdateAssignmentAsync(int id, Assignment assignment)
        {
            var existing = await _assignmentRepository.GetByIdAsync(id);
            if (existing == null)
                return null;

            if (string.IsNullOrWhiteSpace(assignment.Teacher))
                throw new Exception("Teacher is required");
            if (string.IsNullOrWhiteSpace(assignment.Subject))
                throw new Exception("Subject is required");
            if (string.IsNullOrWhiteSpace(assignment.Classroom))
                throw new Exception("Classroom is required");
            if (assignment.StartTime >= assignment.EndTime)
                throw new Exception("Start time must be earlier than end time");

            var classroomConflict = await _assignmentRepository
                .HasClassroomScheduleConflictExcluding(
                    assignment.Classroom,
                    assignment.Day,
                    assignment.StartTime,
                    assignment.EndTime,
                    id);
            if (classroomConflict)
                throw new Exception("The classroom is already occupied at this time");

            var teacherConflict = await _assignmentRepository
                .HasTeacherScheduleConflictExcluding(
                    assignment.Teacher,
                    assignment.Day,
                    assignment.StartTime,
                    assignment.EndTime,
                    id);
            if (teacherConflict)
                throw new Exception(
                    "The teacher already has an assigned class in this time range.");

            existing.Teacher = assignment.Teacher;
            existing.Subject = assignment.Subject;
            existing.Classroom = assignment.Classroom;
            existing.Day = assignment.Day;
            existing.StartTime = assignment.StartTime;
            existing.EndTime = assignment.EndTime;

            await _assignmentRepository.UpdateAsync(existing);
            return existing;
        }

        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            return await _assignmentRepository.DeleteAsync(id);
        }
    }
}
