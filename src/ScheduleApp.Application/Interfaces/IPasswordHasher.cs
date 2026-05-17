using System;

// ScheduleApp.Application/Interfaces/IPasswordHasher.cs
// Contrato para el servicio de hashing de contraseñas.
// Desacopla Application de la librería BCrypt que vive en Infrastructure.
namespace ScheduleApp.Application.Interfaces;

/// Autor:  Mateo Quintero 
/// Version: 0.1

public interface IPasswordHasher
{
    /// <summary>
    /// Verifica si una contraseña en texto plano coincide con su hash almacenado.
    /// </summary>
    /// <param name="password">Contraseña en texto plano ingresada por el usuario.</param>
    /// <param name="hash">Hash almacenado en la base de datos.</param>
    /// <returns>True si coinciden, False si no.</returns>
    bool Verify(string password, string hash);

    /// <summary>
    /// Genera un hash seguro a partir de una contraseña en texto plano.
    /// Usar al crear o actualizar contraseñas de usuarios.
    /// </summary>
    /// <param name="password">Contraseña en texto plano a hashear.</param>
    /// <returns>Hash BCrypt listo para almacenar en BD.</returns>
    string Hash(string password);
}