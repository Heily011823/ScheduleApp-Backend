// Autor: Jacobo
// Version: 0.1
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ScheduleApp.Infrastructure.Data;

namespace ScheduleApp.Infrastructure.Services
{
    // Algoritmo greedy de generacion automatica de horarios.
    //
    // Estrategia:
    //   1) Tomar las materias activas del semestre solicitado.
    //   2) Para cada materia, buscar docentes que la dicten (TeacherSubject).
    //   3) Por cada docente, recorrer su disponibilidad (TeacherAvailability)
    //      e intentar acomodar un bloque de DefaultBlockHours horas.
    //   4) Verificar que el slot no choque con un Schedule existente del docente
    //      y buscar un aula activa que tampoco este ocupada.
    //   5) Si encuentra (docente + aula + slot) crea el Schedule en Draft.
    //   6) Si no, registra un warning con el motivo y pasa a la siguiente materia.
    //
    // Limitaciones conocidas (documentadas en el PR):
    //   - Bloques de tamano fijo (DefaultBlockHours). No reparte las WeeklyHours en
    //     varios dias todavia. Solo programa 1 bloque por materia.
    //   - No filtra materias por programa (el modelo actual no tiene FK Subject->Program).
    //   - No valida capacidad del aula vs estudiantes (el modelo no tiene esa data).
    //   - No respeta reglas TAPSI (es responsabilidad de otra HU).
    public class ScheduleGenerationService : IScheduleGenerationService
    {
        private readonly AppDbContext _context;

        // Bloque por defecto que se intenta asignar a cada materia (en horas).
        private const int DefaultBlockHours = 2;

        public ScheduleGenerationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenerateScheduleResponseDto> GenerateAsync(GenerateScheduleRequestDto request)
        {
            var response = new GenerateScheduleResponseDto();

            // 1) Validar el programa academico
            var program = await _context.AcademicPrograms
                .FirstOrDefaultAsync(p => p.Id == request.AcademicProgramId);

            if (program is null)
            {
                response.Success = false;
                response.Message = $"No se encontro el programa academico con Id '{request.AcademicProgramId}'.";
                return response;
            }

            if (!program.IsActive)
            {
                response.Success = false;
                response.Message = $"El programa '{program.Name}' esta inactivo.";
                return response;
            }

            // 2) Validar rango del semestre
            if (request.SemesterNumber <= 0 || request.SemesterNumber > program.TotalSemesters)
            {
                response.Success = false;
                response.Message = $"El semestre {request.SemesterNumber} esta fuera de rango (1 a {program.TotalSemesters}).";
                return response;
            }

            // 3) Validar jornada
            if (string.IsNullOrWhiteSpace(request.Shift))
            {
                response.Success = false;
                response.Message = "La jornada (Shift) es requerida.";
                return response;
            }

            // 4) Obtener materias activas del semestre
            //    El modelo actual no relaciona Subject con AcademicProgram, asi que
            //    se filtra solo por semester (limitacion conocida).
            var subjects = await _context.Subjects
                .Where(s => s.IsActive && s.Semester == request.SemesterNumber)
                .ToListAsync();

            response.TotalSubjectsRequested = subjects.Count;

            if (subjects.Count == 0)
            {
                response.Success = false;
                response.Message = $"No hay materias activas registradas para el semestre {request.SemesterNumber}.";
                return response;
            }

            // 5) Cargar datos auxiliares en memoria para no consultar la BD en cada iteracion.
            var classrooms = await _context.Classrooms
                .Where(c => c.IsActive)
                .ToListAsync();

            if (classrooms.Count == 0)
            {
                response.Success = false;
                response.Message = "No hay aulas activas registradas en el sistema.";
                return response;
            }

            // Schedules ya existentes (cualquier estado). Sirven para detectar conflictos.
            var existingSchedules = await _context.Schedules.ToListAsync();

            // Lista local de schedules generados en ESTA llamada. Sirve para detectar
            // conflictos contra las propias asignaciones que vamos creando.
            var newSchedules = new List<Schedule>();

            // 6) Recorrer cada materia y tratar de asignarla.
            foreach (var subject in subjects)
            {
                var assigned = await TryScheduleSubjectAsync(
                    subject,
                    program,
                    request,
                    classrooms,
                    existingSchedules,
                    newSchedules,
                    response);

                if (!assigned)
                {
                    // Si no se asigno, el warning ya fue agregado adentro.
                    continue;
                }
            }

            // 7) Persistir los Schedules generados
            if (newSchedules.Count > 0)
            {
                await _context.Schedules.AddRangeAsync(newSchedules);
                await _context.SaveChangesAsync();

                // Refresca con datos completos para el DTO (nombres, etc.).
                foreach (var schedule in newSchedules)
                {
                    var subject = subjects.First(s => s.Id == schedule.SubjectId);
                    var classroom = classrooms.First(c => c.Id == schedule.ClassroomId);
                    var teacher = await _context.Teachers
                        .FirstAsync(t => t.Id == schedule.TeacherId);

                    response.GeneratedSchedules.Add(new GeneratedScheduleEntryDto
                    {
                        Id = schedule.Id,
                        SubjectCode = subject.Code,
                        SubjectName = subject.Name,
                        TeacherFullName = $"{teacher.FirstName} {teacher.LastName}".Trim(),
                        ClassroomCode = classroom.Code,
                        ClassroomName = classroom.Name,
                        Day = schedule.Day,
                        StartTime = schedule.StartTime,
                        EndTime = schedule.EndTime
                    });
                }
            }

            // 8) Construir mensaje final
            response.TotalSubjectsScheduled = newSchedules.Count;
            response.Success = newSchedules.Count > 0;

            if (response.Success)
            {
                response.Message = $"Se generaron {newSchedules.Count} de {subjects.Count} clases para el semestre {request.SemesterNumber} ({program.Name}, {request.Shift}).";
            }
            else
            {
                response.Message = $"No se pudo asignar ninguna clase. Revisa los warnings para mas detalles.";
            }

            return response;
        }

        // Intenta encontrar (docente + aula + slot) para la materia.
        // Devuelve true si lo logro y agrego un Schedule a newSchedules.
        // Devuelve false si fallo y agrego un warning al response.
        private async Task<bool> TryScheduleSubjectAsync(
            Subject subject,
            AcademicProgram program,
            GenerateScheduleRequestDto request,
            List<Classroom> classrooms,
            List<Schedule> existingSchedules,
            List<Schedule> newSchedules,
            GenerateScheduleResponseDto response)
        {
            // Buscar docentes que dictan la materia
            var teacherIds = await _context.TeacherSubjects
                .Where(ts => ts.SubjectId == subject.Id)
                .Select(ts => ts.TeacherId)
                .ToListAsync();

            if (teacherIds.Count == 0)
            {
                response.Warnings.Add($"La materia '{subject.Name}' ({subject.Code}) no tiene docentes asignados.");
                return false;
            }

            // Cargar docentes activos con sus disponibilidades
            var teachers = await _context.Teachers
                .Include(t => t.Availabilities)
                .Where(t => teacherIds.Contains(t.Id) && t.IsActive)
                .ToListAsync();

            if (teachers.Count == 0)
            {
                response.Warnings.Add($"La materia '{subject.Name}' tiene docentes asignados pero ninguno esta activo.");
                return false;
            }

            // Probar cada docente -> cada slot de disponibilidad -> cada aula
            foreach (var teacher in teachers)
            {
                foreach (var availability in teacher.Availabilities)
                {
                    var slotStart = availability.StartTime;
                    var slotEnd = slotStart.Add(TimeSpan.FromHours(DefaultBlockHours));

                    // El bloque debe caber dentro de la franja de disponibilidad
                    if (slotEnd > availability.EndTime)
                        continue;

                    // El docente NO debe estar ocupado en ese slot (existentes + nuevos en esta corrida)
                    var teacherBusy = HasConflict(
                        existingSchedules.Concat(newSchedules),
                        s => s.TeacherId == teacher.Id,
                        availability.Day,
                        slotStart,
                        slotEnd);

                    if (teacherBusy)
                        continue;

                    // Buscar un aula libre en ese mismo slot
                    var freeClassroom = classrooms.FirstOrDefault(classroom =>
                        !HasConflict(
                            existingSchedules.Concat(newSchedules),
                            s => s.ClassroomId == classroom.Id,
                            availability.Day,
                            slotStart,
                            slotEnd));

                    if (freeClassroom is null)
                        continue;

                    // ASIGNADO
                    newSchedules.Add(new Schedule
                    {
                        Id = Guid.NewGuid(),
                        SubjectId = subject.Id,
                        TeacherId = teacher.Id,
                        ClassroomId = freeClassroom.Id,
                        Day = availability.Day,
                        StartTime = slotStart,
                        EndTime = slotEnd,
                        AcademicProgram = program.Name,
                        Shift = request.Shift,
                        Semester = request.SemesterNumber,
                        Status = "Draft",
                        CreatedAt = DateTime.UtcNow
                    });

                    return true;
                }
            }

            response.Warnings.Add($"No se pudo encontrar un slot libre para la materia '{subject.Name}' ({subject.Code}). Verifica disponibilidad de docentes y aulas.");
            return false;
        }

        // Detecta si en la lista de schedules existe alguno que cumpla con el predicado
        // (mismo docente o misma aula) Y que solape con el rango [start, end) en el dia dado.
        // Solapamiento clasico: (A.start < B.end) y (A.end > B.start).
        private static bool HasConflict(
            IEnumerable<Schedule> schedules,
            Func<Schedule, bool> filter,
            DayOfWeek day,
            TimeSpan start,
            TimeSpan end)
        {
            return schedules.Any(s =>
                filter(s) &&
                s.Day == day &&
                s.StartTime < end &&
                s.EndTime > start);
        }
    }
}
