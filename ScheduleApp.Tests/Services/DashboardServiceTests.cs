using FluentAssertions;
using Moq;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.Services;

namespace ScheduleApp.Tests.Services
{
    // Hotfix: el constructor de DashboardService cambio para requerir IDashboardRepository.
    // Se agrega el mock del repositorio para que los tests compilen.
    public class DashboardServiceTests
    {
        private readonly Mock<IDashboardRepository> _repoMock;
        private readonly DashboardService _service;

        public DashboardServiceTests()
        {
            _repoMock = new Mock<IDashboardRepository>();
            _repoMock.Setup(r => r.GetSummary()).Returns(new DashboardSummaryDto
            {
                Subjects = 5,
                Teachers = 3,
                Schedules = 7,
                Programs = 2,
                Classrooms = 4,
                Coordinators = 1
            });
            _service = new DashboardService(_repoMock.Object);
        }

        [Fact]
        public void GetSummary_DeberiaRetornarResumenNoNulo()
        {
            // Act
            var result = _service.GetSummary();

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void GetSummary_DeberiaRetornarValoresMayoresOIgualesACero()
        {
            // Act
            var result = _service.GetSummary();

            // Assert
            result.Subjects.Should().BeGreaterThanOrEqualTo(0);
            result.Teachers.Should().BeGreaterThanOrEqualTo(0);
            result.Schedules.Should().BeGreaterThanOrEqualTo(0);
            result.Programs.Should().BeGreaterThanOrEqualTo(0);
            result.Classrooms.Should().BeGreaterThanOrEqualTo(0);
            result.Coordinators.Should().BeGreaterThanOrEqualTo(0);
        }
        [Fact]
        public void GetSummary_DeberiaRetornarCantidadesCorrectas()
        {
            // Act
            var result = _service.GetSummary();

            // Assert
            result.Subjects.Should().Be(5);
            result.Teachers.Should().Be(3);
            result.Schedules.Should().Be(7);
            result.Programs.Should().Be(2);
            result.Classrooms.Should().Be(4);
            result.Coordinators.Should().Be(1);
        }

        [Fact]
        public void GetSummary_DeberiaRetornarCerosCuandoNoHayRegistros()
        {
            // Arrange
            _repoMock.Setup(r => r.GetSummary()).Returns(new DashboardSummaryDto
            {
                Subjects = 0,
                Teachers = 0,
                Schedules = 0,
                Programs = 0,
                Classrooms = 0,
                Coordinators = 0
            });

            var service = new DashboardService(_repoMock.Object);

            // Act
            var result = service.GetSummary();

            // Assert
            result.Subjects.Should().Be(0);
            result.Teachers.Should().Be(0);
            result.Schedules.Should().Be(0);
            result.Programs.Should().Be(0);
            result.Classrooms.Should().Be(0);
            result.Coordinators.Should().Be(0);
        }

        [Fact]
        public void GetSummary_DeberiaControlarErrores()
        {
            // Arrange
            _repoMock.Setup(r => r.GetSummary())
                .Throws(new Exception("Database error"));

            var service = new DashboardService(_repoMock.Object);

            // Act
            Action act = () => service.GetSummary();

            // Assert
            act.Should().Throw<Exception>()
                .WithMessage("Database error");
        }


    }
}
