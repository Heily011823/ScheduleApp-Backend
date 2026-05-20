using FluentAssertions;
using Moq;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.Services;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Tests.Services
{
    public class TeacherServiceTests
    {
        private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
        private readonly Mock<ISubjectRepository> _subjectRepositoryMock;
        private readonly TeacherService _service;

        public TeacherServiceTests()
        {
            _teacherRepositoryMock = new Mock<ITeacherRepository>();
            _subjectRepositoryMock = new Mock<ISubjectRepository>();

            _service = new TeacherService(
                _teacherRepositoryMock.Object,
                _subjectRepositoryMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_DeberiaCrearDocenteCorrectamente()
        {
            // Arrange
            var dto = new CreateTeacherDto
            {
                FirstName = "Salome",
                LastName = "Carmona",
                Email = "salome@test.com",
                IdentityDocument = "123456",
                PhoneNumber = "3001234567",
                TeachingHours = 20,
                ContractType = "Tiempo Completo",
                Specialties = "Programación"
            };

            _teacherRepositoryMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync((Teacher?)null);

            _teacherRepositoryMock
                .Setup(r => r.GetByIdentityDocumentAsync(dto.IdentityDocument))
                .ReturnsAsync((Teacher?)null);

            _subjectRepositoryMock
                .Setup(r => r.SearchAsync(
                    It.IsAny<string>(),
                    null,
                    true,
                    1,
                    1))
                .ReturnsAsync((
                    new List<Subject>
                    {
                        new Subject
                        {
                            Id = Guid.NewGuid(),
                            Name = "Programación"
                        }
                    },
                    1
                ));

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(dto.Email);

            _teacherRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Teacher>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateAsync_DeberiaLanzarError_SiCorreoExiste()
        {
            // Arrange
            var dto = new CreateTeacherDto
            {
                Email = "duplicado@test.com",
                IdentityDocument = "123"
            };

            _teacherRepositoryMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(new Teacher());

            // Act
            Func<Task> action = async () => await _service.CreateAsync(dto);

            // Assert
            await action.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*correo electrónico ya está registrado*");
        }

        [Fact]
        public async Task CreateAsync_DeberiaLanzarError_SiDocumentoExiste()
        {
            // Arrange
            var dto = new CreateTeacherDto
            {
                Email = "nuevo@test.com",
                IdentityDocument = "999999"
            };

            _teacherRepositoryMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync((Teacher?)null);

            _teacherRepositoryMock
                .Setup(r => r.GetByIdentityDocumentAsync(dto.IdentityDocument))
                .ReturnsAsync(new Teacher());

            // Act
            Func<Task> action = async () => await _service.CreateAsync(dto);

            // Assert
            await action.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*documento de identidad ya está registrado*");
        }

        [Fact]
        public async Task GetByIdAsync_DeberiaRetornarDocente()
        {
            // Arrange
            var teacher = new Teacher
            {
                Id = Guid.NewGuid(),
                FirstName = "Mateo",
                LastName = "Quintero",
                Email = "mateo@test.com",
                IdentityDocument = "123",
                PhoneNumber = "3000000000"
            };

            _teacherRepositoryMock
                .Setup(r => r.GetByIdAsync(teacher.Id))
                .ReturnsAsync(teacher);

            // Act
            var result = await _service.GetByIdAsync(teacher.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be(teacher.Email);
        }

        [Fact]
        public async Task DeleteAsync_DeberiaDesactivarDocente()
        {
            // Arrange
            var teacher = new Teacher
            {
                Id = Guid.NewGuid(),
                IsActive = true
            };

            _teacherRepositoryMock
                .Setup(r => r.GetByIdAsync(teacher.Id))
                .ReturnsAsync(teacher);

            // Act
            var result = await _service.DeleteAsync(teacher.Id);

            // Assert
            result.Should().BeTrue();
            teacher.IsActive.Should().BeFalse();

            _teacherRepositoryMock.Verify(
                r => r.UpdateAsync(It.IsAny<Teacher>()),
                Times.Once
            );
        }

        [Fact]
        public async Task ChangeStatusAsync_DeberiaCambiarEstado()
        {
            // Arrange
            var teacher = new Teacher
            {
                Id = Guid.NewGuid(),
                IsActive = true
            };

            _teacherRepositoryMock
                .Setup(r => r.GetByIdAsync(teacher.Id))
                .ReturnsAsync(teacher);

            // Act
            var result = await _service.ChangeStatusAsync(teacher.Id, false);

            // Assert
            result.Should().NotBeNull();
            result!.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task SearchAsync_DeberiaRetornarDocentes()
        {
            // Arrange
            var teachers = new List<Teacher>
            {
                new Teacher
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Salome",
                    LastName = "Carmona",
                    Email = "salome@test.com"
                }
            };

            _teacherRepositoryMock
                .Setup(r => r.SearchAsync(null, null))
                .ReturnsAsync(teachers);

            // Act
            var result = await _service.SearchAsync(null, null, null);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
        }
    }
}