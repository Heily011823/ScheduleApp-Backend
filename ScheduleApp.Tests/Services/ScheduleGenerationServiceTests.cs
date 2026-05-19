using FluentAssertions;
using Moq;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.Services;

namespace ScheduleApp.Tests.Services;

// Autor: Jacobo
// Version: 0.1
// Pruebas unitarias del ScheduleGenerationService (HU-169).
// Cubren los 7 casos de exito y los 6 casos de error definidos en la HU.
// El service refactorizado por el equipo delega la logica de generacion al
// IScheduleRepository, por lo que las validaciones de aula, cruces y reglas
// especiales se prueban verificando que el service delega correctamente al
// repositorio (que es responsable de aplicar esas reglas).
public class ScheduleGenerationServiceTests
{
    private readonly Mock<IScheduleRepository> _scheduleRepoMock;
    private readonly ScheduleGenerationService _service;

    public ScheduleGenerationServiceTests()
    {
        _scheduleRepoMock = new Mock<IScheduleRepository>();
        _service = new ScheduleGenerationService(_scheduleRepoMock.Object);
    }

    // ============================================================
    // CASOS DE EXITO (7)
    // ============================================================

    // CDA 1: Generar horario con datos validos.
    [Fact]
    public async Task GenerateAsync_WithValidData_ShouldReturnSuccessResponse()
    {
        var request = BuildValidRequest();
        var schedules = BuildSampleSchedules(3);
        _scheduleRepoMock.Setup(r => r.AcademicProgramExistsAsync(request.AcademicProgramId))
                         .ReturnsAsync(true);
        _scheduleRepoMock.Setup(r => r.GetSubjectsForGenerationAsync(
                request.AcademicProgramId, request.SemesterNumber, request.Shift))
            .ReturnsAsync(schedules);

        var result = await _service.GenerateAsync(request);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("correctamente");
    }

    // CDA 2: Los horarios generados se retornan listos para guardar.
    [Fact]
    public async Task GenerateAsync_WhenSchedulesGenerated_ShouldReturnAllInResponse()
    {
        var request = BuildValidRequest();
        var schedules = BuildSampleSchedules(3);
        _scheduleRepoMock.Setup(r => r.AcademicProgramExistsAsync(It.IsAny<Guid>()))
                         .ReturnsAsync(true);
        _scheduleRepoMock.Setup(r => r.GetSubjectsForGenerationAsync(
                It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(schedules);

        var result = await _service.GenerateAsync(request);

        result.GeneratedSchedules.Should().HaveCount(3);
        result.GeneratedSchedules.Should().BeEquivalentTo(schedules);
    }

    // CDA 3: La respuesta trae la metadata correcta de cantidad de materias.
    [Fact]
    public async Task GenerateAsync_ShouldReturnCorrectMetadata()
    {
        var request = BuildValidRequest();
        var schedules = BuildSampleSchedules(5);
        _scheduleRepoMock.Setup(r => r.AcademicProgramExistsAsync(It.IsAny<Guid>()))
                         .ReturnsAsync(true);
        _scheduleRepoMock.Setup(r => r.GetSubjectsForGenerationAsync(
                It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(schedules);

        var result = await _service.GenerateAsync(request);

        result.TotalSubjectsRequested.Should().Be(5);
        result.TotalSubjectsScheduled.Should().Be(5);
    }

    // CDA 4: La validacion de disponibilidad de aula se delega al repositorio.
    // El service llama al repo con los filtros correctos para que aplique esa logica.
    [Fact]
    public async Task GenerateAsync_ShouldDelegateAvailabilityCheckToRepository()
    {
        var request = BuildValidRequest();
        _scheduleRepoMock.Setup(r => r.AcademicProgramExistsAsync(It.IsAny<Guid>()))
                         .ReturnsAsync(true);
        _scheduleRepoMock.Setup(r => r.GetSubjectsForGenerationAsync(
                It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(BuildSampleSchedules(1));

        await _service.GenerateAsync(request);

        _scheduleRepoMock.Verify(r => r.GetSubjectsForGenerationAsync(
            request.AcademicProgramId,
            request.SemesterNumber,
            request.Shift), Times.Once);
    }

    // CDA 5: La validacion de cruce de docentes se delega al repositorio.
    // El test verifica que el service llama exactamente una vez al repo con
    // los parametros correctos para que el repo aplique esa validacion.
    [Fact]
    public async Task GenerateAsync_ShouldDelegateCrossCheckToRepository()
    {
        var request = BuildValidRequest();
        _scheduleRepoMock.Setup(r => r.AcademicProgramExistsAsync(It.IsAny<Guid>()))
                         .ReturnsAsync(true);
        _scheduleRepoMock.Setup(r => r.GetSubjectsForGenerationAsync(
                It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(BuildSampleSchedules(2));

        await _service.GenerateAsync(request);

        _scheduleRepoMock.Verify(r => r.GetSubjectsForGenerationAsync(
            It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
    }

    // CDA 6: Las reglas especiales (TAPSI, etc.) se aplican en el repositorio.
    // El service permite y delega esas reglas; aqui validamos que la llamada se hace.
    [Fact]
    public async Task GenerateAsync_ShouldDelegateSpecialRulesToRepository()
    {
        var request = BuildValidRequest();
        request.Shift = "Nocturna";
        _scheduleRepoMock.Setup(r => r.AcademicProgramExistsAsync(It.IsAny<Guid>()))
                         .ReturnsAsync(true);
        _scheduleRepoMock.Setup(r => r.GetSubjectsForGenerationAsync(
                It.IsAny<Guid>(), It.IsAny<int>(), "Nocturna"))
            .ReturnsAsync(BuildSampleSchedules(1));

        await _service.GenerateAsync(request);

        _scheduleRepoMock.Verify(r => r.GetSubjectsForGenerationAsync(
            It.IsAny<Guid>(), It.IsAny<int>(), "Nocturna"), Times.Once);
    }

    // CDA 7: Cuando el proceso finaliza correctamente, retorna una respuesta valida.
    [Fact]
    public async Task GenerateAsync_WhenSuccessful_ShouldReturnNonNullValidResponse()
    {
        var request = BuildValidRequest();
        _scheduleRepoMock.Setup(r => r.AcademicProgramExistsAsync(It.IsAny<Guid>()))
                         .ReturnsAsync(true);
        _scheduleRepoMock.Setup(r => r.GetSubjectsForGenerationAsync(
                It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(BuildSampleSchedules(1));

        var result = await _service.GenerateAsync(request);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().NotBeNullOrWhiteSpace();
        result.GeneratedSchedules.Should().NotBeNull();
        result.Warnings.Should().NotBeNull();
    }

    // ============================================================
    // CASOS DE ERROR (6)
    // ============================================================

    // Error 1: No se genera horario sin AcademicProgramId.
    [Fact]
    public async Task GenerateAsync_WithEmptyProgramId_ShouldReturnError()
    {
        var request = new GenerateScheduleRequestDto
        {
            AcademicProgramId = Guid.Empty,
            SemesterNumber = 1,
            Shift = "Diurna"
        };

        var result = await _service.GenerateAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("programa");
        _scheduleRepoMock.Verify(r => r.GetSubjectsForGenerationAsync(
            It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    // Error 2 (continuacion): No se genera con SemesterNumber invalido.
    [Fact]
    public async Task GenerateAsync_WithInvalidSemester_ShouldReturnError()
    {
        var request = new GenerateScheduleRequestDto
        {
            AcademicProgramId = Guid.NewGuid(),
            SemesterNumber = 0,
            Shift = "Diurna"
        };

        var result = await _service.GenerateAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("semestre");
    }

    // Error 3 (continuacion): No se genera sin Shift.
    [Fact]
    public async Task GenerateAsync_WithEmptyShift_ShouldReturnError()
    {
        var request = new GenerateScheduleRequestDto
        {
            AcademicProgramId = Guid.NewGuid(),
            SemesterNumber = 1,
            Shift = "   "
        };

        var result = await _service.GenerateAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("jornada");
    }

    // Error 4: Cuando el programa no existe en BD se controla con mensaje claro.
    [Fact]
    public async Task GenerateAsync_WhenProgramNotExists_ShouldReturnError()
    {
        var request = BuildValidRequest();
        _scheduleRepoMock.Setup(r => r.AcademicProgramExistsAsync(It.IsAny<Guid>()))
                         .ReturnsAsync(false);

        var result = await _service.GenerateAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("no existe");
        _scheduleRepoMock.Verify(r => r.GetSubjectsForGenerationAsync(
            It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    // Error 5: Si el repositorio lanza, el service propaga la excepcion al controller.
    [Fact]
    public async Task GenerateAsync_WhenRepositoryThrows_ShouldPropagateError()
    {
        var request = BuildValidRequest();
        _scheduleRepoMock.Setup(r => r.AcademicProgramExistsAsync(It.IsAny<Guid>()))
                         .ReturnsAsync(true);
        _scheduleRepoMock.Setup(r => r.GetSubjectsForGenerationAsync(
                It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("DB connection error"));

        Func<Task> act = async () => await _service.GenerateAsync(request);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    // Error 6: Cuando no es posible generar (sin materias) la respuesta es controlada.
    [Fact]
    public async Task GenerateAsync_WhenNoSubjectsFound_ShouldReturnErrorWithWarning()
    {
        var request = BuildValidRequest();
        _scheduleRepoMock.Setup(r => r.AcademicProgramExistsAsync(It.IsAny<Guid>()))
                         .ReturnsAsync(true);
        _scheduleRepoMock.Setup(r => r.GetSubjectsForGenerationAsync(
                It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(new List<GeneratedScheduleEntryDto>());

        var result = await _service.GenerateAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().NotBeNullOrWhiteSpace();
        result.Warnings.Should().NotBeEmpty();
        result.GeneratedSchedules.Should().BeEmpty();
    }

    // ============================================================
    // HELPERS
    // ============================================================

    private static GenerateScheduleRequestDto BuildValidRequest()
    {
        return new GenerateScheduleRequestDto
        {
            AcademicProgramId = Guid.NewGuid(),
            SemesterNumber = 1,
            Shift = "Diurna"
        };
    }

    private static List<GeneratedScheduleEntryDto> BuildSampleSchedules(int count)
    {
        var schedules = new List<GeneratedScheduleEntryDto>();
        for (int i = 0; i < count; i++)
        {
            schedules.Add(new GeneratedScheduleEntryDto
            {
                Id = Guid.NewGuid()
            });
        }
        return schedules;
    }
}
