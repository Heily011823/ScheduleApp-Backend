using ScheduleApp.Application.DTOs;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Interfaces;

public interface ISubjectService
{
    Task<bool> DeleteSubjectAsync(Guid id);

    Task CreateSubjectAsync(CreateSubjectDto dto);

    Task UpdateSubjectAsync(Guid id, UpdateSubjectDto dto);

    // Detalle de materia por Id (HU-120). Retorna null si no existe.
    Task<Subject?> GetSubjectByIdAsync(Guid id);

    // MODIFICADO: Cambiamos el tipo de retorno al DTO de Jacobo
    // y agregamos los parámetros numéricos obligatorios para la paginación.
    Task<PagedResultDto<Subject>> SearchSubjectsAsync(
        string? search,
        int? semester,
        bool? isActive,
        int page,
        int pageSize
    );
    Task<byte[]> ExportSubjectsToExcelAsync();
    Task<byte[]> ExportSubjectsToPdfAsync();
}
