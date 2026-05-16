// Autor: Jacobo
// Version: 0.1
// Envoltorio generico para respuestas paginadas de la API.
// Lo usan los endpoints que retornan listas con metadata de paginacion.

namespace ScheduleApp.Application.DTOs;

public class PagedResultDto<T>
{
    // Los registros de la pagina actual.
    public IEnumerable<T> Items { get; set; } = new List<T>();

    // Numero de pagina actual (empezando en 1).
    public int Page { get; set; }

    // Cuantos registros por pagina.
    public int PageSize { get; set; }

    // Cantidad total de registros en todas las paginas.
    public int TotalCount { get; set; }

    // Cantidad total de paginas (calculado).
    public int TotalPages =>
        PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    // Indica si hay pagina anterior.
    public bool HasPreviousPage => Page > 1;

    // Indica si hay pagina siguiente.
    public bool HasNextPage => Page < TotalPages;
}
