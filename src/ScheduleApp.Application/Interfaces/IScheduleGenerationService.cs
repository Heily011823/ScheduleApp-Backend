using ScheduleApp.Application.DTOs;

namespace ScheduleApp.Application.Interfaces
{
    // Servicio responsable de generar automaticamente un horario academico
    // para un programa, semestre y jornada dados.
    public interface IScheduleGenerationService
    {
        Task<GenerateScheduleResponseDto> GenerateAsync(GenerateScheduleRequestDto request);
    }
}
