// Autor: Jacobo
// Version: 0.1

using System.Threading.Tasks;
using ScheduleApp.Application.DTOs;

namespace ScheduleApp.Application.Interfaces
{
    // Servicio responsable de generar automáticamente un horario académico
    // para un programa, semestre y jornada dados.
    public interface IScheduleGenerationService
    {
        Task<GenerateScheduleResponseDto> GenerateAsync(
            GenerateScheduleRequestDto request
        );
    }
}