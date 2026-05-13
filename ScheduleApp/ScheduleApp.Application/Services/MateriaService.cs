using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
namespace ScheduleApp.Application.Services;
/// <summary>
/// Servicio de materias. Por ahora delega directamente al repositorio.
/// Se mantiene esta capa para preservar la arquitectura (Controller -> Service -> Repository)
/// y permitir agregar logica de negocio mas adelante sin tocar el controller.
/// </summary>
public class MateriaService : IMateriaService
{
    private readonly IMateriaRepository _materiaRepository;

    public MateriaService(IMateriaRepository materiaRepository)
    {
        _materiaRepository = materiaRepository;
    }

    public async Task<IEnumerable<Materia>> SearchMateriasAsync(
        string? nombre,
        int? semestre,
        bool? isActive)
    {
        return await _materiaRepository.SearchMateriasAsync(
            nombre,
            semestre,
            isActive);
    }
}