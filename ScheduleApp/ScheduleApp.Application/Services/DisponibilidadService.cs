using ScheduleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Application.Services
{
    public class DisponibilidadService
    {
        private List<Asignacion> asignaciones = new List<Asignacion>();

        public bool EstaDisponible(string docente, DateTime fechaHora)
        {
            return !asignaciones.Any(a =>
             a.Docente == docente && a.FechaHora == fechaHora);
        }
        public string GuardarAsignacion(Asignacion nueva)
        {
            if (!EstaDisponible(nueva.Docente, nueva.FechaHora))
            {
                return "El docente ya tiene una clase en ese horario";
            }

            asignaciones.Add(nueva);
            return "Asignacion guardada correctamente";
        }
    }
}
