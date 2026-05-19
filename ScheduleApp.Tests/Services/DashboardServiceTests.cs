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
    }
}
