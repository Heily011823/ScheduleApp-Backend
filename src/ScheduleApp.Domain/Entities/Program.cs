// Autor: Jacobo
// Version: 0.1
using System;

namespace ScheduleApp.Domain.Entities
{
    // Programa academico (ej. Ingenieria de Sistemas, Medicina, etc.).
    // Cada programa tiene un codigo unico y un nombre. Las materias
    // a futuro deberian pertenecer a un programa, pero esa relacion
    // aun no esta definida en el modelo.
    public class Program
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
