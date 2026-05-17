using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task CreateSubjectAsync(CreateSubjectDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new Exception("Code is required");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Name is required");

            if (dto.Semester <= 0)
                throw new Exception("Semester must be greater than 0");

            if (dto.Credits <= 0)
                throw new Exception("Credits must be greater than 0");

            if (dto.WeeklyHours <= 0)
                throw new Exception("Weekly hours must be greater than 0");

            var existingSubject = await _subjectRepository.GetByCodeAsync(dto.Code);

            if (existingSubject != null)
                throw new Exception("Subject code already exists");

            var subject = new Subject
            {
                Id = Guid.NewGuid(),
                Code = dto.Code,
                Name = dto.Name,
                Semester = dto.Semester,
                Credits = dto.Credits,
                WeeklyHours = dto.WeeklyHours,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _subjectRepository.CreateAsync(subject);
        }

        public async Task UpdateSubjectAsync(Guid id, UpdateSubjectDto dto)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);

            if (subject == null)
                throw new Exception("Subject not found");

            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new Exception("Code is required");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Name is required");

            if (dto.Semester <= 0)
                throw new Exception("Semester must be greater than 0");

            if (dto.Credits <= 0)
                throw new Exception("Credits must be greater than 0");

            if (dto.WeeklyHours <= 0)
                throw new Exception("Weekly hours must be greater than 0");

            subject.Code = dto.Code;
            subject.Name = dto.Name;
            subject.Semester = dto.Semester;
            subject.Credits = dto.Credits;
            subject.WeeklyHours = dto.WeeklyHours;
            subject.UpdatedAt = DateTime.UtcNow;

            await _subjectRepository.UpdateAsync(subject);
        }

        public async Task<bool> DeleteSubjectAsync(Guid id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);

            if (subject == null)
                throw new Exception("Subject not found");

            if (!subject.IsActive)
                throw new Exception("The subject has already been deleted");

            subject.IsActive = false;

            await _subjectRepository.UpdateAsync(subject);

            return true;
        }

      
        public async Task<PagedResultDto<Subject>> SearchSubjectsAsync(
            string? search,
            int? semester,
            bool? isActive,
            int page,
            int pageSize)
        {
            // Consume la tupla del repositorio de manera limpia usando las variables correctas
            var (items, totalCount) = await _subjectRepository.SearchAsync(
                search,
                semester,
                isActive,
                page,
                pageSize);

            // Mapea la información al envoltorio genérico de la aplicación
            return new PagedResultDto<Subject>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}