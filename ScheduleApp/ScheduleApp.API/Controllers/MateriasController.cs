using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
namespace ScheduleApp.API.Controllers
{
    /// <summary>
    /// Controlador de materias. Expone los endpoints REST para consultar
    /// las materias academicas registradas en el sistema.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MateriasController : ControllerBase
    {
        private readonly IMateriaService _materiaService;

        public MateriasController(IMateriaService materiaService)
        {
            _materiaService = materiaService;
        }

        /// <summary>
        /// Obtiene el listado de materias. Soporta filtros opcionales
        /// por nombre (busqueda parcial), semestre y estado activo.
        /// Si no hay coincidencias retorna una lista vacia.
        /// </summary>
        /// <returns>
        /// 200 OK con la lista de materias (puede estar vacia);
        /// 500 si ocurre un error interno al consultar la base de datos.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Materia>>> GetMaterias(
            [FromQuery] string? nombre,
            [FromQuery] int? semestre,
            [FromQuery] bool? isActive)
        {
            try
            {
                var materias = await _materiaService.SearchMateriasAsync(
                    nombre,
                    semestre,
                    isActive);
                return Ok(materias);
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}