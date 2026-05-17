using System;

/// Autor:  Mateo Quintero 
/// Version: 0.1

// ScheduleApp.Infrastructure/Services/PasswordHasherService.cs
// Implementación concreta de IPasswordHasher usando la librería BCrypt.Net-Next.
// Al vivir en Infrastructure, mantiene a Application libre de dependencias externas.
namespace ScheduleApp.Infrastructure.Services;

using ScheduleApp.Application.Interfaces;

/// <summary>
/// Servicio de hashing de contraseñas con algoritmo BCrypt.
/// BCrypt es resistente a ataques de fuerza bruta por su factor de costo configurable.
/// </summary>
public class PasswordHasherService : IPasswordHasher
{
    /// <summary>
    /// Verifica si la contraseña en texto plano coincide con el hash BCrypt almacenado.
    /// BCrypt.Verify maneja internamente la extracción del salt del hash.
    /// </summary>
    public bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);

    /// <summary>
    /// Genera un hash BCrypt de la contraseña. Cada llamada produce un hash
    /// diferente gracias al salt aleatorio incorporado automáticamente.
    /// </summary>
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);
}