using FluentAssertions;
using Moq;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.Services;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Tests.Services
{
    public class ClassroomServiceTests
    {
        private readonly Mock<IClassroomRepository> _repositoryMock;
        private readonly ClassroomService _service;

        public ClassroomServiceTests()
        {
            _repositoryMock = new Mock<IClassroomRepository>();
            _service = new ClassroomService(_repositoryMock.Object);
        }

        [Fact]
        public async Task CreateClassroomAsync_ShouldCreateClassroom_WhenDataIsValid()
        {
            // Arrange
            var classroom = new Classroom
            {
                Id = Guid.NewGuid(),
                Code = "A101",
                Name = "Sala 101",
                Building = "Bloque A",
                Floor = 1,
                Capacity = 30,
                Type = "Teórica",
                IsActive = true
            };

            _repositoryMock
                .Setup(r => r.GetByCodeAsync(classroom.Code))
                .ReturnsAsync((Classroom?)null);

            // Act
            await _service.CreateClassroomAsync(classroom);

            // Assert
            _repositoryMock.Verify(r =>
                r.CreateAsync(It.IsAny<Classroom>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateClassroomAsync_ShouldThrowException_WhenCodeAlreadyExists()
        {
            // Arrange
            var classroom = new Classroom
            {
                Id = Guid.NewGuid(),
                Code = "A101",
                Name = "Sala Duplicada"
            };

            _repositoryMock
                .Setup(r => r.GetByCodeAsync(classroom.Code))
                .ReturnsAsync(classroom);

            // Act
            Func<Task> action = async () =>
                await _service.CreateClassroomAsync(classroom);

            // Assert
            await action.Should()
                .ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GetClassroomsAsync_ShouldReturnClassroomList()
        {
            // Arrange
            var classrooms = new List<Classroom>
            {
                new Classroom
                {
                    Id = Guid.NewGuid(),
                    Code = "A101",
                    Name = "Sala 101"
                },
                new Classroom
                {
                    Id = Guid.NewGuid(),
                    Code = "B202",
                    Name = "Sala 202"
                }
            };

            _repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(classrooms);

            // Act
            var result = await _service.GetClassroomsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetClassroomByIdAsync_ShouldReturnClassroom_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();

            var classroom = new Classroom
            {
                Id = id,
                Code = "A101",
                Name = "Sala 101"
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(classroom);

            // Act
            var result = await _service.GetClassroomByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
        }

        [Fact]
        public async Task UpdateClassroomAsync_ShouldUpdateClassroom_WhenValid()
        {
            // Arrange
            var classroom = new Classroom
            {
                Id = Guid.NewGuid(),
                Code = "A101",
                Name = "Sala Actualizada"
            };

            _repositoryMock
                .Setup(r => r.GetByCodeAsync(classroom.Code))
                .ReturnsAsync((Classroom?)null);

            // Act
            await _service.UpdateClassroomAsync(classroom);

            // Assert
            _repositoryMock.Verify(r =>
                r.UpdateAsync(It.IsAny<Classroom>()),
                Times.Once);
        }

        [Fact]
        public async Task ChangeStatusAsync_ShouldChangeClassroomStatus()
        {
            // Arrange
            var id = Guid.NewGuid();

            var classroom = new Classroom
            {
                Id = id,
                Code = "A101",
                Name = "Sala 101",
                IsActive = true
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(classroom);

            // Act
            var result = await _service.ChangeStatusAsync(id, false);

            // Assert
            result.Should().NotBeNull();
            result!.IsActive.Should().BeFalse();

            _repositoryMock.Verify(r =>
                r.UpdateAsync(It.IsAny<Classroom>()),
                Times.Once);
        }

        [Fact]
        public async Task ChangeStatusAsync_ShouldReturnNull_WhenClassroomDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Classroom?)null);

            // Act
            var result = await _service.ChangeStatusAsync(id, false);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteClassroomAsync_ShouldCallDeleteAsync()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            await _service.DeleteClassroomAsync(id);

            // Assert
            _repositoryMock.Verify(r =>
                r.DeleteAsync(id),
                Times.Once);
        }
    }
}