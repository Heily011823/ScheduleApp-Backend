using ScheduleApp.Application.DTOs;
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

        public async Task<AssignmentResponseDto> CreateAssignmentAsync(CreateAssignmentDto dto)
        {
            ValidateAssignmentData(
                dto.Teacher,
                dto.Subject,
                dto.Classroom,
                dto.StartTime,
                dto.EndTime);

            var assignment = new Assignment
            {
                Teacher = dto.Teacher,
                Subject = dto.Subject,
                Classroom = dto.Classroom,
                Day = dto.Day,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            await _assignmentRepository.CreateAsync(assignment);

            return MapToResponse(assignment);
        }

        public async Task<List<AssignmentResponseDto>> GetAssignmentsAsync()
        {
            var assignments = await _assignmentRepository.GetAllAsync();
            return assignments.Select(MapToResponse).ToList();
        }

        public async Task<AssignmentResponseDto?> GetAssignmentByIdAsync(int id)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(id);
            return assignment is null ? null : MapToResponse(assignment);
        }

        public async Task<AssignmentResponseDto?> UpdateAssignmentAsync(int id, UpdateAssignmentDto dto)
        {
            var existing = await _assignmentRepository.GetByIdAsync(id);
            if (existing is null)
                return null;

            ValidateAssignmentData(
                dto.Teacher,
                dto.Subject,
                dto.Classroom,
                dto.StartTime,
                dto.EndTime);

            existing.Teacher = dto.Teacher;
            existing.Subject = dto.Subject;
            existing.Classroom = dto.Classroom;
            existing.Day = dto.Day;
            existing.StartTime = dto.StartTime;
            existing.EndTime = dto.EndTime;

            await _assignmentRepository.UpdateAsync(existing);

            return MapToResponse(existing);
        }

        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            return await _assignmentRepository.DeleteAsync(id);
        }

        // Valida los campos obligatorios y el rango horario.
        // Lanza Exception cuando algun dato es invalido.
        private static void ValidateAssignmentData(
            string teacher,
            string subject,
            string classroom,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            if (string.IsNullOrWhiteSpace(teacher))
                throw new Exception("Teacher is required");

            if (string.IsNullOrWhiteSpace(subject))
                throw new Exception("Subject is required");

            if (string.IsNullOrWhiteSpace(classroom))
                throw new Exception("Classroom is required");

            if (startTime >= endTime)
                throw new Exception("Start time must be earlier than end time");
        }

        // Convierte la entidad de dominio en su DTO de respuesta.
        private static AssignmentResponseDto MapToResponse(Assignment assignment)
        {
            return new AssignmentResponseDto
            {
                Id = assignment.Id,
                Teacher = assignment.Teacher,
                Subject = assignment.Subject,
                Classroom = assignment.Classroom,
                Day = assignment.Day,
                StartTime = assignment.StartTime,
                EndTime = assignment.EndTime
            };
        }
    }
}
