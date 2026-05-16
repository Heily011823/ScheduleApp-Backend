// Autor: Jacobo
// Version: 0.1
using System;

namespace ScheduleApp.Application.DTOs
{
    public class ProgramResponseDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
