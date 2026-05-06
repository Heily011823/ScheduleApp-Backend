using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace ScheduleApp.Domain.Entities
{
    public class Asignacion
    {
        public int Id { get; set; }
        public string Docente { get; set; }
        public string Materia { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
