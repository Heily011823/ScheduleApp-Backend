using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using ClosedXML.Excel;
using System.IO;
using System.Text;

namespace ScheduleApp.Application.Services
{
    public class SubjectService : ISubjectService
    {
        private const int PageSize = 1000;
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        // Detalle de materia por Id (HU-120).
        // Reutiliza GetByIdAsync del repo que ya existe.
        public async Task<Subject?> GetSubjectByIdAsync(Guid id)
        {
            return await _subjectRepository.GetByIdAsync(id);
        }

        public async Task<byte[]> ExportSubjectsToExcelAsync()
        {
            var (subjects, totalCount) = await _subjectRepository.SearchAsync(
            null,
            null,
            null,
            1,
            1000
            );

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Materias");

                worksheet.Cell(1, 1).Value = "Código";
                worksheet.Cell(1, 2).Value = "Nombre";
                worksheet.Cell(1, 3).Value = "Semestre";
                worksheet.Cell(1, 4).Value = "Créditos";
                worksheet.Cell(1, 5).Value = "Horas Semanales";
                worksheet.Cell(1, 6).Value = "Estado";

                var headerRange = worksheet.Range(1, 1, 1, 6);
                headerRange.Style.Font.Bold = true;

                int row = 2;

                foreach (var subject in subjects)
                {
                    worksheet.Cell(row, 1).Value = subject.Code;
                    worksheet.Cell(row, 2).Value = subject.Name;
                    worksheet.Cell(row, 3).Value = subject.Semester;
                    worksheet.Cell(row, 4).Value = subject.Credits;
                    worksheet.Cell(row, 5).Value = subject.WeeklyHours;
                    worksheet.Cell(row, 6).Value = subject.IsActive
                        ? "Activa"
                        : "Inactiva";
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<byte[]> ExportSubjectsToPdfAsync()
        {
            var subjects = new List<Subject>
            {
                new Subject
                {
                    Code = "103004",
                    Name = "Teoría de Sistemas",
                    Semester = 1,
                    Credits = 3,
                    WeeklyHours = 3,
                    IsActive = true
                },
                new Subject
                {
                    Code = "103026",
                    Name = "Redes LAN",
                    Semester = 5,
                    Credits = 3,
                    WeeklyHours = 4,
                    IsActive = true
                }
            };

            var html = new StringBuilder();
            html.AppendLine("<html><body>");
            html.AppendLine("<h1>Reporte de Materias</h1>");
            html.AppendLine(
                "<table border='1' cellpadding='6' cellspacing='0' style='border-collapse:collapse;width:100%'>"
            );
            html.AppendLine("<thead><tr>");
            html.AppendLine(
                "<th>Código</th>" +
                "<th>Nombre</th>" +
                "<th>Semestre</th>" +
                "<th>Créditos</th>" +
                "<th>Horas Semanales</th>" +
                "<th>Estado</th>"
            );
            html.AppendLine("</tr></thead><tbody>");

            foreach (var subject in subjects)
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{subject.Code}</td>");
                html.AppendLine($"<td>{subject.Name}</td>");
                html.AppendLine($"<td>{subject.Semester}</td>");
                html.AppendLine($"<td>{subject.Credits}</td>");
                html.AppendLine($"<td>{subject.WeeklyHours}</td>");
                html.AppendLine(
                    $"<td>{(subject.IsActive ? "Activa" : "Inactiva")}</td>"
                );
                html.AppendLine("</tr>");
            }

            html.AppendLine("</tbody></table></body></html>");
            return Encoding.UTF8.GetBytes(html.ToString());
        }

        public async Task CreateSubjectAsync(CreateSubjectDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new Exception("Code is required");
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Name is required");
            if (dto.Semester <= 0)
                throw new Exception("Semester must be greater than 0");
            if (dto.Credits <= 0)
                throw new Exception("Credits must be greater than 0");
            if (dto.WeeklyHours <= 0)
                throw new Exception("Weekly hours must be greater than 0");

            var existingSubject = await _subjectRepository.GetByCodeAsync(dto.Code);
            if (existingSubject != null)
                throw new Exception("Subject code already exists");

            var subject = new Subject
            {
                Id = Guid.NewGuid(),
                Code = dto.Code,
                Name = dto.Name,
                Semester = dto.Semester,
                Credits = dto.Credits,
                WeeklyHours = dto.WeeklyHours,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _subjectRepository.CreateAsync(subject);
        }

        public async Task UpdateSubjectAsync(Guid id, UpdateSubjectDto dto)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
                throw new Exception("Subject not found");
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new Exception("Code is required");
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Name is required");
            if (dto.Semester <= 0)
                throw new Exception("Semester must be greater than 0");
            if (dto.Credits <= 0)
                throw new Exception("Credits must be greater than 0");
            if (dto.WeeklyHours <= 0)
                throw new Exception("Weekly hours must be greater than 0");

            subject.Code = dto.Code;
            subject.Name = dto.Name;
            subject.Semester = dto.Semester;
            subject.Credits = dto.Credits;
            subject.WeeklyHours = dto.WeeklyHours;
            subject.IsActive = dto.IsActive;
            subject.UpdatedAt = DateTime.UtcNow;

            await _subjectRepository.UpdateAsync(subject);
        }

        public async Task<bool> DeleteSubjectAsync(Guid id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
                throw new Exception("Subject not found");
            if (!subject.IsActive)
                throw new Exception("The subject has already been deleted");

            subject.IsActive = false;
            await _subjectRepository.UpdateAsync(subject);
            return true;
        }

        public async Task<PagedResultDto<Subject>> SearchSubjectsAsync(
            string? search,
            int? semester,
            bool? isActive,
            int page,
            int pageSize)
        {
            var (items, totalCount) = await _subjectRepository.SearchAsync(
                search,
                semester,
                isActive,
                page,
                pageSize);

            return new PagedResultDto<Subject>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
