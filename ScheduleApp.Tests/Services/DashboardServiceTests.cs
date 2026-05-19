using FluentAssertions;
using ScheduleApp.Application.Services;
using Xunit;

namespace ScheduleApp.Tests.Services
{
    public class DashboardServiceTests
    {
        [Fact]
        public void GetSummary_DeberiaRetornarResumenNoNulo()
        {
            // Arrange
            var service = new DashboardService();

            // Act
            var result = service.GetSummary();

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void GetSummary_DeberiaRetornarValoresMayoresOIgualesACero()
        {
            // Arrange
            var service = new DashboardService();

            // Act
            var result = service.GetSummary();

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