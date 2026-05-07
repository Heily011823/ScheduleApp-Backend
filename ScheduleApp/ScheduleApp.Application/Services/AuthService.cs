using ScheduleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleApp.Application.Services
{
    public class AuthService
    {
        public bool TienePermiso(Usuario usuario, string modulo)
        {

            if (usuario.Rol == Rol.Administrador)
                return true;


            if (usuario.Rol == Rol.Coordinador && modulo == "Asignaciones")
                return true;


            return false;
        }
    }
}

