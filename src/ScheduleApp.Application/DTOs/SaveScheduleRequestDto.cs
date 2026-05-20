// Autor: Jacobo
// Version: 0.1

using System.Collections.Generic;

namespace ScheduleApp.Application.DTOs
{
    public class SaveScheduleRequestDto
    {
        public List<GeneratedScheduleEntryDto> Schedules { get; set; } = new();
    }
}