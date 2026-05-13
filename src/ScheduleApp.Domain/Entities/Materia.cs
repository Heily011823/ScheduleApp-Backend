using System;
namespace ScheduleApp.Domain.Entities
{
    /// <summary>
    /// Entidad que representa una materia académica del sistema.
    /// Contiene la información básica del plan de estudios: código,
    /// nombre, semestre, créditos y carga horaria semanal.
    /// </summary>
    /// Autor: Jacobo
    /// Version: 0.1
    public class Materia
    {
        /// <summary>Identificador único de la materia (GUID).</summary>
        public Guid Id { get; set; }

        /// <summary>Código único de la materia (ej. "MAT101", "ING201").</summary>
        public string Codigo { get; set; } = string.Empty;

        /// <summary>Nombre completo de la materia.</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Semestre al que pertenece la materia (ej. 1, 2, 3, ...).</summary>
        public int Semestre { get; set; }

        /// <summary>Créditos académicos que otorga la materia.</summary>
        public int Creditos { get; set; }

        /// <summary>Horas de clase a la semana.</summary>
        public int HorasSemanales { get; set; }

        /// <summary>
        /// Indica si la materia está activa en el plan de estudios.
        /// Permite soft delete: materias inactivas no se eliminan, solo se ocultan.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Fecha de creación del registro (UTC).</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}