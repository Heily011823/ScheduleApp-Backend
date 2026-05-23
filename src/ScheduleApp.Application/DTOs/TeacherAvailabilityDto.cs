using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Application.DTOs
{
    /// <summary>
    /// DTO para representar la disponibilidad horaria de un docente
    /// </summary>
    public class TeacherAvailabilityDto
    {
        public Guid Id { get; set; }
        public Guid TeacherId { get; set; }
        public DayOfWeek Day { get; set; }
        public string DayName => GetDayNameInSpanish(Day);
        public TimeSpan StartTime { get; set; }
        public string StartTimeFormatted => StartTime.ToString(@"hh\:mm");
        public TimeSpan EndTime { get; set; }
        public string EndTimeFormatted => EndTime.ToString(@"hh\:mm");
        public int MaxTeachingHours { get; set; }

        private string GetDayNameInSpanish(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => "Lunes",
                DayOfWeek.Tuesday => "Martes",
                DayOfWeek.Wednesday => "Miércoles",
                DayOfWeek.Thursday => "Jueves",
                DayOfWeek.Friday => "Viernes",
                DayOfWeek.Saturday => "Sábado",
                DayOfWeek.Sunday => "Domingo",
                _ => day.ToString()
            };
        }
    }
}
