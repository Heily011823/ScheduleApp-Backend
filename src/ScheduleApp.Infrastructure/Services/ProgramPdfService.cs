// Autor: Jacobo
// Version: 0.1
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ScheduleApp.Application.Interfaces;

namespace ScheduleApp.Infrastructure.Services
{
    // Genera reportes PDF de programas usando QuestPDF.
    // La plantilla tiene un header con titulo y fecha, una tabla con
    // Codigo y Nombre con filas alternadas, y un footer con paginacion.
    public class ProgramPdfService : IProgramPdfService
    {
        private readonly IProgramRepository _repository;

        public ProgramPdfService(IProgramRepository repository)
        {
            _repository = repository;
        }

        public async Task<byte[]> GenerateProgramsListAsync()
        {
            var programs = await _repository.GetAllAsync();
            var generatedAt = DateTime.UtcNow;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("Listado de Programas Academicos")
                            .FontSize(20).Bold().FontColor(Colors.Grey.Darken3);

                        col.Item().PaddingTop(2).Text($"Generado el {generatedAt:dd/MM/yyyy HH:mm} UTC")
                            .FontSize(10).FontColor(Colors.Grey.Medium);
                    });

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        if (programs.Count == 0)
                        {
                            col.Item().AlignCenter().PaddingTop(40)
                                .Text("No hay programas registrados.")
                                .FontSize(12).Italic().FontColor(Colors.Grey.Medium);
                            return;
                        }

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(5);
                            });

                            // Encabezado de la tabla
                            table.Header(header =>
                            {
                                header.Cell()
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(8)
                                    .Text("Codigo").Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(8)
                                    .Text("Nombre").Bold();
                            });

                            // Filas con efecto zebra
                            for (int i = 0; i < programs.Count; i++)
                            {
                                var program = programs[i];
                                var background = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                                table.Cell()
                                    .Background(background)
                                    .Padding(8)
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Text(program.Code);

                                table.Cell()
                                    .Background(background)
                                    .Padding(8)
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Text(program.Name);
                            }
                        });

                        col.Item().PaddingTop(15).AlignRight()
                            .Text($"Total: {programs.Count} programa(s)")
                            .FontSize(10).Italic().FontColor(Colors.Grey.Medium);
                    });

                    page.Footer().AlignRight().Text(text =>
                    {
                        text.Span("Pagina ").FontSize(9).FontColor(Colors.Grey.Medium);
                        text.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Medium);
                        text.Span(" de ").FontSize(9).FontColor(Colors.Grey.Medium);
                        text.TotalPages().FontSize(9).FontColor(Colors.Grey.Medium);
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
