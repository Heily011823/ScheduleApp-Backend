namespace ScheduleApp.Application.Interfaces
{
    // Servicio encargado de generar reportes PDF relacionados con programas academicos.
    public interface IProgramPdfService
    {
        // Genera un PDF con el listado completo de programas en una tabla (Codigo, Nombre).
        // Devuelve el archivo como arreglo de bytes para que el controller lo retorne como descarga.
        Task<byte[]> GenerateProgramsListAsync();
    }
}
