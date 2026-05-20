using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

namespace ScheduleApp.Application.Services
{
    public class ScheduleGenerationService : IScheduleGenerationService, IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleGenerationService(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        // ============================================================
        // GENERACIÓN AUTOMÁTICA DE HORARIO
        // ============================================================
        public async Task<GenerateScheduleResponseDto> GenerateAsync(GenerateScheduleRequestDto request)
        {
            var response = new GenerateScheduleResponseDto();

            if (request.AcademicProgramId == Guid.Empty)
            {
                response.Success = false;
                response.Message = "Debe seleccionar un programa académico válido.";
                return response;
            }

            if (request.SemesterNumber <= 0)
            {
                response.Success = false;
                response.Message = "Debe seleccionar un semestre válido.";
                return response;
            }

            if (string.IsNullOrWhiteSpace(request.Shift))
            {
                response.Success = false;
                response.Message = "Debe seleccionar una jornada válida.";
                return response;
            }

            bool programExists = await _scheduleRepository.AcademicProgramExistsAsync(request.AcademicProgramId);
            if (!programExists)
            {
                response.Success = false;
                response.Message = "El programa académico seleccionado no existe.";
                return response;
            }

            var generatedSchedules = await _scheduleRepository.GetSubjectsForGenerationAsync(
                request.AcademicProgramId,
                request.SemesterNumber,
                request.Shift
            );

            response.TotalSubjectsRequested = generatedSchedules.Count;
            response.TotalSubjectsScheduled = generatedSchedules.Count;
            response.GeneratedSchedules = generatedSchedules;

            if (generatedSchedules.Count == 0)
            {
                response.Success = false;
                response.Message = "No se encontraron materias activas para generar el horario con los filtros seleccionados.";
                response.Warnings.Add("Verifique que existan materias activas para el semestre seleccionado.");
                return response;
            }

            response.Success = true;
            response.Message = "Horario generado correctamente.";
            return response;
        }

        // ============================================================
        // GUARDAR HORARIOS EN LA BASE DE DATOS
        // ============================================================
        public async Task SaveAsync(SaveScheduleRequestDto request)
        {
           
            var schedulesToSave = request.Schedules;
            await _scheduleRepository.SaveAsync(schedulesToSave);
        }

        // ============================================================
        // OBTENER HORARIOS FILTRADOS
        // ============================================================
        public async Task<List<GeneratedScheduleEntryDto>> GetByFiltersAsync(string academicProgram, string shift, int semester)
        {
            return await _scheduleRepository.GetByFiltersAsync(academicProgram, shift, semester);
        }
    }
}