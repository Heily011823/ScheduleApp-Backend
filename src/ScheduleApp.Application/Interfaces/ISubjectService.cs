using ScheduleApp.Application.DTOs;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces;

public interface ISubjectService
{
    Task<bool> DeleteSubjectAsync(Guid id);

    Task CreateSubjectAsync(CreateSubjectDto dto);

    Task UpdateSubjectAsync(Guid id, UpdateSubjectDto dto);

    Task<Subject?> GetSubjectByIdAsync(Guid id);

    Task<List<Subject>> SearchSubjectsAsync(
    string? search,
    int? semester,
    bool? isActive
    );
}
