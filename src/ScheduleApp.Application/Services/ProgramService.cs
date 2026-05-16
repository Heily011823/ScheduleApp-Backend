using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Application.Services
{
    public class ProgramService : IProgramService
    {
        private readonly IProgramRepository _repository;

        public ProgramService(IProgramRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProgramResponseDto>> GetAllAsync()
        {
            var programs = await _repository.GetAllAsync();
            return programs.Select(MapToResponse).ToList();
        }

        public async Task<ProgramResponseDto?> GetByIdAsync(Guid id)
        {
            var program = await _repository.GetByIdAsync(id);
            return program is null ? null : MapToResponse(program);
        }

        public async Task<ProgramResponseDto> CreateAsync(CreateProgramDto dto)
        {
            ValidateBasicFields(dto.Code, dto.Name);

            if (await _repository.ExistsByCodeAsync(dto.Code))
                throw new InvalidOperationException($"Ya existe un programa con el codigo '{dto.Code}'.");

            var program = new Program
            {
                Id = Guid.NewGuid(),
                Code = dto.Code.Trim(),
                Name = dto.Name.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(program);

            return MapToResponse(program);
        }

        public async Task<ProgramResponseDto?> UpdateAsync(Guid id, UpdateProgramDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
                return null;

            ValidateBasicFields(dto.Code, dto.Name);

            if (await _repository.ExistsByCodeAsync(dto.Code, excludeId: id))
                throw new InvalidOperationException($"Ya existe otro programa con el codigo '{dto.Code}'.");

            existing.Code = dto.Code.Trim();
            existing.Name = dto.Name.Trim();
            existing.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existing);

            return MapToResponse(existing);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            // TODO(equipo): cuando se defina la relacion entre Program y horarios (Assignment/Subject)
            // agregar aqui la validacion que impida borrar un programa si tiene horarios asociados.
            // El modelo actual no tiene ninguna entidad con FK a Program, asi que la validacion
            // del CDA #3 ("no permitir borrar si esta amarrado a un horario") no se puede implementar
            // todavia. Cuando se agregue la FK basta con consultar el repositorio correspondiente
            // y devolver false (o lanzar excepcion) si existen registros asociados.

            return await _repository.DeleteAsync(id);
        }

        private static void ValidateBasicFields(string code, string name)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("El codigo del programa es requerido.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del programa es requerido.");
        }

        private static ProgramResponseDto MapToResponse(Program program)
        {
            return new ProgramResponseDto
            {
                Id = program.Id,
                Code = program.Code,
                Name = program.Name,
                CreatedAt = program.CreatedAt,
                UpdatedAt = program.UpdatedAt
            };
        }
    }
}
