using ClosedXML.Excel;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;
using System.IO;
using System.Text;

namespace ScheduleApp.Application.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
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
                IsDeleted = false,
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
            subject.UpdatedAt = DateTime.UtcNow;

            await _subjectRepository.UpdateAsync(subject);
        }

        public async Task<bool> DeleteSubjectAsync(Guid id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);

            if (subject == null)
                throw new Exception("Subject not found");

            if (subject.IsDeleted)
                throw new Exception("The subject has already been deleted");

            // ✅ Eliminación lógica
            subject.IsDeleted = true;
            subject.UpdatedAt = DateTime.UtcNow;

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

        public async Task<Subject?> GetSubjectByIdAsync(Guid id)
        {
            return await _subjectRepository.GetByIdAsync(id);
        }

        public async Task<byte[]> ExportSubjectsToExcelAsync()
        {
            var subjects = await _subjectRepository.GetActiveAsync();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Materias");

            sheet.Cell(1, 1).Value = "Código";
            sheet.Cell(1, 2).Value = "Nombre";
            sheet.Cell(1, 3).Value = "Semestre";
            sheet.Cell(1, 4).Value = "Créditos";
            sheet.Cell(1, 5).Value = "Horas semanales";
            sheet.Cell(1, 6).Value = "Estado";

            for (int i = 0; i < subjects.Count; i++)
            {
                var s = subjects[i];
                sheet.Cell(i + 2, 1).Value = s.Code;
                sheet.Cell(i + 2, 2).Value = s.Name;
                sheet.Cell(i + 2, 3).Value = s.Semester;
                sheet.Cell(i + 2, 4).Value = s.Credits;
                sheet.Cell(i + 2, 5).Value = s.WeeklyHours;
                sheet.Cell(i + 2, 6).Value = s.IsActive ? "Activo" : "Inactivo";
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // ✅ PDF como HTML simple, sin iTextSharp
        public async Task<byte[]> ExportSubjectsToPdfAsync()
        {
            var subjects = await _subjectRepository.GetActiveAsync();

            var sb = new StringBuilder();
            sb.AppendLine("<html><body>");
            sb.AppendLine("<h1>Reporte de Materias</h1>");
            sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0'>");
            sb.AppendLine("<tr><th>Código</th><th>Nombre</th><th>Semestre</th><th>Créditos</th><th>Horas semanales</th><th>Estado</th></tr>");

            foreach (var s in subjects)
            {
                sb.AppendLine($"<tr><td>{s.Code}</td><td>{s.Name}</td><td>{s.Semester}</td><td>{s.Credits}</td><td>{s.WeeklyHours}</td><td>{(s.IsActive ? "Activo" : "Inactivo")}</td></tr>");
            }

            sb.AppendLine("</table></body></html>");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}