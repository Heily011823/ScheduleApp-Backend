using ScheduleApp.Domain.Entities;
namespace ScheduleApp.Application.Interfaces;
/// <summary>
/// Contrato del servicio de gestion de materias. Define las operaciones
/// de consulta expuestas a la capa API.
/// </summary>
public interface IMateriaService
{
    /// <summary>
    /// Obtiene materias aplicando filtros opcionales (nombre, semestre, estado).
    /// Si no se envia ningun filtro, retorna todas las materias registradas.
    /// </summary>
    Task<IEnumerable<Materia>> SearchMateriasAsync(
        string? nombre,
        int? semestre,
        bool? isActive);
}