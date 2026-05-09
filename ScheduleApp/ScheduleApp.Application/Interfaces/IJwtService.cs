using System;

// ScheduleApp.Application/Interfaces/IJwtService.cs
// Contrato para el servicio de generación de tokens JWT.
// La implementación concreta vive en ScheduleApp.Infrastructure.

/// Autor:  Mateo Quintero 
/// Version: 0.1

namespace ScheduleApp.Application.Interfaces;

using ScheduleApp.Domain.Entities;

public interface IJwtService
{
    /// <summary>
    /// Genera un token JWT firmado con los claims del usuario dado.
    /// </summary>
    /// <param name="user">Usuario autenticado del que se extraen Id, Email, Nombre y Rol.</param>
    /// <returns>Token JWT como string listo para enviar al cliente.</returns>
    string GenerateToken(User user);

    /// <summary>
    /// Calcula la fecha y hora de expiración del token desde el momento actual.
    /// </summary>
    /// <returns>DateTime en UTC con la expiración.</returns>
    DateTime GetExpiration();
}