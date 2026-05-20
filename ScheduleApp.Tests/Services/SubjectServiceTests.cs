using FluentAssertions;
using Moq;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.Services;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Tests.Services
{
    public class SubjectServiceTests
    {
        private readonly Mock<ISubjectRepository> _repoMock;
        private readonly SubjectService _service;

        public SubjectServiceTests()
        {
            _repoMock = new Mock<ISubjectRepository>();
            _service = new SubjectService(_repoMock.Object);
        }

        [Fact]
        public async Task CreateSubjectAsync_DeberiaCrearMateriaCorrectamente()
        {
            // Arrange
            var dto = new CreateSubjectDto
            {
                Code = "MAT101",
                Name = "Matemáticas",
                Semester = 1,
                Credits = 3,
                WeeklyHours = 4
            };

            _repoMock.Setup(r => r.GetByCodeAsync(dto.Code))
                .ReturnsAsync((Subject?)null);

            // Act
            await _service.CreateSubjectAsync(dto);

            // Assert
            _repoMock.Verify(r =>
                r.CreateAsync(It.IsAny<Subject>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateSubjectAsync_NoDebePermitirCodigoDuplicado()
        {
            // Arrange
            var dto = new CreateSubjectDto
            {
                Code = "MAT101",
                Name = "Matemáticas",
                Semester = 1,
                Credits = 3,
                WeeklyHours = 4
            };

            _repoMock.Setup(r => r.GetByCodeAsync(dto.Code))
                .ReturnsAsync(new Subject());

            // Act
            Func<Task> act = async () =>
                await _service.CreateSubjectAsync(dto);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Subject code already exists");
        }

        [Fact]
        public async Task CreateSubjectAsync_NoDebePermitirCamposVacios()
        {
            // Arrange
            var dto = new CreateSubjectDto
            {
                Code = "",
                Name = "",
                Semester = 0,
                Credits = 0,
                WeeklyHours = 0
            };

            // Act
            Func<Task> act = async () =>
                await _service.CreateSubjectAsync(dto);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task GetSubjectByIdAsync_DeberiaRetornarMateria()
        {
            // Arrange
            var subjectId = Guid.NewGuid();

            _repoMock.Setup(r => r.GetByIdAsync(subjectId))
                .ReturnsAsync(new Subject
                {
                    Id = subjectId,
                    Code = "MAT101",
                    Name = "Matemáticas"
                });

            // Act
            var result = await _service.GetSubjectByIdAsync(subjectId);

            // Assert
            result.Should().NotBeNull();
            result!.Code.Should().Be("MAT101");
        }

        [Fact]
        public async Task GetSubjectByIdAsync_DeberiaControlarMateriaInexistente()
        {
            // Arrange
            var subjectId = Guid.NewGuid();

            _repoMock.Setup(r => r.GetByIdAsync(subjectId))
                .ReturnsAsync((Subject?)null);

            // Act
            var result = await _service.GetSubjectByIdAsync(subjectId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateSubjectAsync_DeberiaActualizarMateria()
        {
            // Arrange
            var subjectId = Guid.NewGuid();

            var subject = new Subject
            {
                Id = subjectId,
                Code = "OLD",
                Name = "Viejo"
            };

            var dto = new UpdateSubjectDto
            {
                Code = "NEW",
                Name = "Nuevo",
                Semester = 2,
                Credits = 4,
                WeeklyHours = 5,
                IsActive = true
            };

            _repoMock.Setup(r => r.GetByIdAsync(subjectId))
                .ReturnsAsync(subject);

            // Act
            await _service.UpdateSubjectAsync(subjectId, dto);

            // Assert
            _repoMock.Verify(r =>
                r.UpdateAsync(It.IsAny<Subject>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateSubjectAsync_DeberiaControlarMateriaInexistente()
        {
            // Arrange
            var subjectId = Guid.NewGuid();

            var dto = new UpdateSubjectDto
            {
                Code = "NEW",
                Name = "Nuevo",
                Semester = 2,
                Credits = 4,
                WeeklyHours = 5,
                IsActive = true
            };

            _repoMock.Setup(r => r.GetByIdAsync(subjectId))
                .ReturnsAsync((Subject?)null);

            // Act
            Func<Task> act = async () =>
                await _service.UpdateSubjectAsync(subjectId, dto);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Subject not found");
        }

        [Fact]
        public async Task DeleteSubjectAsync_DeberiaDesactivarMateria()
        {
            // Arrange
            var subjectId = Guid.NewGuid();

            var subject = new Subject
            {
                Id = subjectId,
                IsActive = true
            };

            _repoMock.Setup(r => r.GetByIdAsync(subjectId))
                .ReturnsAsync(subject);

            // Act
            var result = await _service.DeleteSubjectAsync(subjectId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task SearchSubjectsAsync_DeberiaRetornarMaterias()
        {
            // Arrange
            var subjects = new List<Subject>
            {
                new Subject
                {
                    Code = "MAT101",
                    Name = "Matemáticas"
                }
            };

            _repoMock.Setup(r =>
                r.SearchAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<bool?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync((subjects, 1));

            // Act
            var result = await _service.SearchSubjectsAsync(
                "MAT",
                null,
                true,
                1,
                10);

            // Assert
            result.Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateSubjectAsync_DeberiaControlarErroresRepositorio()
        {
            // Arrange
            var dto = new CreateSubjectDto
            {
                Code = "MAT101",
                Name = "Matemáticas",
                Semester = 1,
                Credits = 3,
                WeeklyHours = 4
            };

            _repoMock.Setup(r => r.GetByCodeAsync(dto.Code))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            Func<Task> act = async () =>
                await _service.CreateSubjectAsync(dto);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Database error");
        }
    }
}