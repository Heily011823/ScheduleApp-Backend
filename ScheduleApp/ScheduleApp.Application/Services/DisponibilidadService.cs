using ScheduleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleApp.Application.Services
{
    public class DisponibilidadService
    {
        private List<Asignacion> asignaciones = new List<Asignacion>();

        public string GuardarAsignacion(Asignacion nueva)
        {
            bool conflictoDocente = asignaciones.Any(a =>
                a.Docente == nueva.Docente &&
                a.Dia == nueva.Dia &&
                nueva.HoraInicio < a.HoraFin &&
                nueva.HoraFin > a.HoraInicio
            );

            if (conflictoDocente)
            {
                return "El docente ya tiene una clase en ese horario";
            }

            bool conflictoAula = asignaciones.Any(a =>
                a.Aula == nueva.Aula &&
                a.Dia == nueva.Dia &&
                nueva.HoraInicio < a.HoraFin &&
                nueva.HoraFin > a.HoraInicio
            );

            if (conflictoAula)
            {
                return "El aula ya está ocupada en ese horario";
            }

            asignaciones.Add(nueva);

            return "Asignación guardada correctamente";
        }
    }
}