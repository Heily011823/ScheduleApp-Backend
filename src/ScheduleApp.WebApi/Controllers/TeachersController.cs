using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;

namespace ScheduleApp.WebApi.Controllers
{
    /// <summary>
    /// Controlador para gestión de docentes del sistema académico.
    /// Ruta base: /api/teachers
    /// </summary>

    /// Autor:  Mateo Quintero 
    /// Version: 0.1
    /// rama: 103-docentes-por-documento
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeachersController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        /// <summary>
        /// Obtiene todos los docentes con filtros opcionales.
        /// 🔍 Búsqueda por: documento, nombre, correo, especialidad, tipo contrato, estado
        /// </summary>
        /// <param name="document">Número de documento (búsqueda parcial)</param>
        /// <param name="name">Nombre o apellido (búsqueda parcial)</param>
        /// <param name="email">Correo electrónico (búsqueda parcial)</param>
        /// <param name="specialty">Especialidad/materia (búsqueda parcial)</param>
        /// <param name="contractType">Tipo de contrato: Tiempo Completo, Cátedra, Medio Tiempo</param>
        /// <param name="status">Estado: active, inactive, all</param>
        /// <param name="page">Número de página (default: 1)</param>
        /// <param name="pageSize">Tamaño de página (default: 10, max: 50)</param>
        /// <returns>Lista filtrada de docentes</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? document = null,
            [FromQuery] string? name = null,
            [FromQuery] string? email = null,
            [FromQuery] string? specialty = null,
            [FromQuery] string? contractType = null,
            [FromQuery] string? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validar parámetros de paginación
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 50) pageSize = 50;

                var (teachers, totalCount) = await _teacherService.SearchAdvancedWithPaginationAsync(
                    document, name, email, specialty, contractType, status, page, pageSize);

                var response = new
                {
                    data = teachers,
                    pagination = new
                    {
                        currentPage = page,
                        pageSize = pageSize,
                        totalCount = totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    },
                    filters = new
                    {
                        document = string.IsNullOrEmpty(document) ? "Todos" : document,
                        name = string.IsNullOrEmpty(name) ? "Todos" : name,
                        email = string.IsNullOrEmpty(email) ? "Todos" : email,
                        specialty = string.IsNullOrEmpty(specialty) ? "Todas" : specialty,
                        contractType = string.IsNullOrEmpty(contractType) ? "Todos" : contractType,
                        status = string.IsNullOrEmpty(status) ? "Todos" : status
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al consultar docentes en la base de datos.",
                    detail = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Búsqueda rápida (autocompletado) para sugerencias en tiempo real
        /// </summary>
        /// <param name="term">Término de búsqueda (busca en documento, nombre, correo)</param>
        /// <param name="limit">Máximo de resultados (default: 5)</param>
        [HttpGet("search/quick")]
        public async Task<IActionResult> QuickSearch(
            [FromQuery] string term,
            [FromQuery] int limit = 5)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                {
                    return Ok(new { suggestions = new List<object>() });
                }

                var suggestions = await _teacherService.QuickSearchAsync(term, limit);
                return Ok(new
                {
                    term = term,
                    suggestions = suggestions,
                    count = suggestions.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error en búsqueda rápida", detail = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un docente específico por su ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var teacher = await _teacherService.GetByIdAsync(id);
                if (teacher is null)
                {
                    return NotFound(new
                    {
                        message = $"No se encontró un docente con el ID '{id}'.",
                        timestamp = DateTime.UtcNow
                    });
                }
                return Ok(teacher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al consultar el docente.",
                    detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene un docente por documento de identidad
        /// </summary>
        [HttpGet("document/{document}")]
        public async Task<IActionResult> GetByDocument(string document)
        {
            try
            {
                var teacher = await _teacherService.GetByIdentityDocumentAsync(document);
                if (teacher is null)
                {
                    return NotFound(new
                    {
                        message = $"No se encontró un docente con el documento '{document}'."
                    });
                }
                return Ok(teacher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al consultar el docente por documento.",
                    detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene un docente por correo electrónico
        /// </summary>
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                var teacher = await _teacherService.GetByEmailAsync(email);
                if (teacher is null)
                {
                    return NotFound(new
                    {
                        message = $"No se encontró un docente con el correo '{email}'."
                    });
                }
                return Ok(teacher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al consultar el docente por correo.",
                    detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene todos los docentes activos (útiles para asignaciones)
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveTeachers()
        {
            try
            {
                var teachers = await _teacherService.GetActiveTeachersAsync();
                return Ok(new
                {
                    count = teachers.Count(),
                    data = teachers
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al consultar docentes activos.",
                    detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene estadísticas de docentes (dashboard)
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var stats = await _teacherService.GetStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al obtener estadísticas.",
                    detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Crea un nuevo docente
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTeacherDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    message = "Datos inválidos",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });

            try
            {
                var teacher = await _teacherService.CreateAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = teacher.Id },
                    new
                    {
                        message = "Docente creado exitosamente",
                        teacher = teacher
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al crear el docente.",
                    detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Actualiza la información de un docente existente
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeacherDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var teacher = await _teacherService.UpdateAsync(id, dto);
                if (teacher is null)
                {
                    return NotFound(new
                    {
                        message = $"No se encontró un docente con el ID '{id}'."
                    });
                }
                return Ok(new
                {
                    message = "Docente actualizado exitosamente",
                    teacher = teacher
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al actualizar el docente.",
                    detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Desactiva un docente (eliminación lógica)
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _teacherService.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = $"No se encontró un docente con el ID '{id}'."
                    });
                }
                return Ok(new
                {
                    message = "Docente desactivado exitosamente",
                    teacherId = id,
                    status = "inactive"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al eliminar el docente.",
                    detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Activa un docente previamente desactivado
        /// </summary>
        [HttpPatch("{id:guid}/activate")]
        public async Task<IActionResult> Activate(Guid id)
        {
            try
            {
                var teacher = await _teacherService.ChangeStatusAsync(id, true);
                if (teacher is null)
                    return NotFound(new
                    {
                        message = $"No se encontró un docente con el ID '{id}'."
                    });
                return Ok(new
                {
                    message = "Docente activado exitosamente",
                    teacher = teacher
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al activar el docente.",
                    detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Cambia el estado del docente (activo/inactivo)
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var teacher = await _teacherService.ChangeStatusAsync(id, dto.IsActive);
                if (teacher is null)
                    return NotFound(new
                    {
                        message = $"No se encontró un docente con el ID '{id}'."
                    });
                return Ok(new
                {
                    message = dto.IsActive ? "Docente activado" : "Docente desactivado",
                    teacher = teacher
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al cambiar el estado del docente.",
                    detail = ex.Message
                });
            }
        }



        /// <summary>
        /// Obtiene tipos de contrato disponibles (para filtros)
        /// </summary>
        [HttpGet("metadata/contract-types")]
        public async Task<IActionResult> GetContractTypes()
        {
            var contractTypes = new[] { "Tiempo Completo", "Medio Tiempo", "Cátedra", "No asignado" };
            return Ok(new { contractTypes });
        }


        /// <summary>
        /// Obtiene el horario/schedule de un docente específico
        /// </summary>
        /// <param name="id">ID del docente</param>
        /// <returns>Lista de disponibilidades horarias del docente</returns>
        [HttpGet("{id:guid}/schedule")]
        public async Task<IActionResult> GetTeacherSchedule(Guid id)
        {
            try
            {
                var schedule = await _teacherService.GetTeacherScheduleAsync(id);

                if (schedule == null || !schedule.Any())
                {
                    return Ok(new
                    {
                        message = "El docente no tiene horarios configurados",
                        schedule = new List<TeacherAvailabilityDto>()
                    });
                }

                return Ok(new
                {
                    teacherId = id,
                    schedule = schedule,
                    totalBlocks = schedule.Count()
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al obtener horario del docente",
                    detail = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }


        /// <summary>
        /// Obtiene especialidades disponibles desde la base de datos
        /// </summary>
        [HttpGet("metadata/specialties")]
        public async Task<IActionResult> GetSpecialties()
        {
            try
            {
                var specialties = await _teacherService.GetAllSpecialtiesAsync();

                if (!specialties.Any())
                {
                    return Ok(new
                    {
                        specialties = new List<SpecialtyDto>(),
                        message = "No hay especialidades cargadas. Use POST /api/teachers/metadata/specialties/seed para cargar datos iniciales.",
                        hasData = false
                    });
                }

                return Ok(new
                {
                    specialties = specialties,
                    count = specialties.Count(),
                    hasData = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al obtener especialidades",
                    detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Endpoint para cargar especialidades por defecto (solo usar una vez)
        /// </summary>
        /*
        [HttpPost("metadata/specialties/seed")]
        public async Task<IActionResult> SeedSpecialties()
        {

            var existing = await _teacherService.GetAllSpecialtiesAsync();
            if (existing.Any())
                return BadRequest(new { message = "Las especialidades ya fueron cargadas. No se puede ejecutar el seed nuevamente." });

            try
            {
                var defaultSpecialties = new[]
                {
            new { Name = "Lenguas Extranjeras", Description = "Idiomas y lingüística", DisplayOrder = 1 },
            new { Name = "Matemáticas", Description = "Matemáticas puras y aplicadas", DisplayOrder = 2 },
            new { Name = "Humanísticas", Description = "Ciencias humanas y sociales", DisplayOrder = 3 },
            new { Name = "Física", Description = "Física teórica y experimental", DisplayOrder = 4 },
            new { Name = "Ética", Description = "Ética y moral", DisplayOrder = 5 },
            new { Name = "Arquitectura de Software", Description = "Diseño de software", DisplayOrder = 6 },
            new { Name = "Electrónica Digital", Description = "Circuitos digitales", DisplayOrder = 7 },
            new { Name = "Ingeniería de Software", Description = "Desarrollo de software", DisplayOrder = 8 },
            new { Name = "Backend", Description = "Desarrollo del lado del servidor", DisplayOrder = 9 },
            new { Name = "Frontend", Description = "Desarrollo del lado del cliente", DisplayOrder = 10 },
            new { Name = "Ingeniería de Datos", Description = "Procesamiento de datos", DisplayOrder = 11 }
        };

                var result = await _teacherService.SeedSpecialtiesAsync(defaultSpecialties);

                return Ok(new
                {
                    message = "Especialidades cargadas exitosamente",
                    added = result.Added,
                    skipped = result.Skipped,
                    total = result.Total
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al cargar especialidades",
                    detail = ex.Message
                });
            }
        }*/

    }
}