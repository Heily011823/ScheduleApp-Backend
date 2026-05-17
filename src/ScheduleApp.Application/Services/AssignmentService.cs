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

            await _assignmentRepository.CreateAsync(assignment);
        }

        public async Task<List<Assignment>> GetAssignmentsAsync()
        {
            return await _assignmentRepository.GetAllAsync();
        }
    }
}