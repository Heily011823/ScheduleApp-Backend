// Autor: Jacobo
// Version: 0.1
namespace ScheduleApp.Application.DTOs
{
    // DTO para editar un programa academico existente (HU-127 PUT).
    // No incluye TotalSemesters porque cambiarlo afectaria los ProgramSemesters ya creados.
    // Si se necesita cambiar el numero de semestres, eliminar y recrear el programa.
    public class UpdateAcademicProgramDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Shift { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
