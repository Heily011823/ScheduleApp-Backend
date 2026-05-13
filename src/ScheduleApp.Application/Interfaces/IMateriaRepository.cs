using ScheduleApp.Domain.Entities;
namespace ScheduleApp.Application.Interfaces;
/// <summary>
/// Contrato para el acceso a datos de materias. Permite que Application
/// consulte materias sin depender de EF Core directamente.
/// </summary>
public interface IMateriaRepository
{
    /// <summary>
    /// Busca materias aplicando filtros opcionales. Si todos los filtros
    /// vienen nulos, retorna todas las materias registradas.
    /// </summary>
    /// <param name="nombre">Filtro parcial por nombre (contiene).</param>
    /// <param name="semestre">Filtro exacto por numero de semestre.</param>
    /// <param name="isActive">Filtro por estado activo o inactivo.</param>
    /// <returns>Lista de materias que cumplen los filtros, o vacia si no hay coincidencias.</returns>
    Task<IEnumerable<Materia>> SearchMateriasAsync(
        string? nombre,
        int? semestre,
        bool? isActive);
}