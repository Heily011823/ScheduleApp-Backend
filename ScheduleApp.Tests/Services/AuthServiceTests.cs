using FluentAssertions;
using Moq;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.Services;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Tests.Services;

// Autor: Jacobo
// Version: 0.2
// Pruebas unitarias del AuthService (HU-167).
// Patron AAA (Arrange, Act, Assert) aplicado en cada test.
public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPasswordHasher> _hasherMock;
    private readonly Mock<IJwtService> _jwtMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _hasherMock = new Mock<IPasswordHasher>();
        _jwtMock = new Mock<IJwtService>();
        _service = new AuthService(
            _userRepoMock.Object,
            _jwtMock.Object,
            _hasherMock.Object);
    }

    // CDA 1: Usuario con credenciales correctas puede iniciar sesion.
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnLoginResponse()
    {
        // Arrange
        var request = new LoginRequestDto { Login = "admin@uam.edu.co", Password = "Password123" };
        var user = BuildActiveUser();
        _userRepoMock.Setup(r => r.GetByEmailOrUsernameAsync(request.Login)).ReturnsAsync(user);
        _hasherMock.Setup(h => h.Verify(request.Password, user.PasswordHash)).Returns(true);
        _jwtMock.Setup(j => j.GenerateToken(user)).Returns("fake-jwt-token");
        _jwtMock.Setup(j => j.GetExpiration()).Returns(DateTime.UtcNow.AddMinutes(60));

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
    }

    // CDA 2: El servicio retorna una respuesta de autenticacion valida.
    [Fact]
    public async Task LoginAsync_WhenSuccessful_ShouldReturnNonNullValidResponse()
    {
        // Arrange
        var request = BuildValidRequest();
        var user = BuildActiveUser();
        SetupSuccessfulLogin(user);

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.UserName.Should().NotBeNullOrWhiteSpace();
        result.Role.Should().NotBeNullOrWhiteSpace();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    // CDA 3: Se genera correctamente la informacion de sesion / token.
    [Fact]
    public async Task LoginAsync_WhenSuccessful_ShouldReturnAccessTokenAndExpiration()
    {
        // Arrange
        var request = BuildValidRequest();
        var user = BuildActiveUser();
        var expectedToken = "generated-jwt-12345";
        var expectedExpiration = DateTime.UtcNow.AddMinutes(60);
        _userRepoMock.Setup(r => r.GetByEmailOrUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);
        _hasherMock.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        _jwtMock.Setup(j => j.GenerateToken(user)).Returns(expectedToken);
        _jwtMock.Setup(j => j.GetExpiration()).Returns(expectedExpiration);

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        result.AccessToken.Should().Be(expectedToken);
        result.ExpiresAt.Should().Be(expectedExpiration);
        _jwtMock.Verify(j => j.GenerateToken(user), Times.Once);
    }

    // CDA 4: Se retornan los datos basicos del usuario autenticado.
    [Fact]
    public async Task LoginAsync_WhenSuccessful_ShouldReturnUserNameAndRole()
    {
        // Arrange
        var request = BuildValidRequest();
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Jacobo Henao",
            Email = "jhenao@uam.edu.co",
            Username = "jhenao",
            PasswordHash = "hashed",
            IsActive = true,
            Role = new Role { Id = Guid.NewGuid(), Name = "Administrador" }
        };
        SetupSuccessfulLogin(user);

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        result.UserName.Should().Be("Jacobo Henao");
        result.Role.Should().Be("Administrador");
    }

    // Error 1: No se permite iniciar sesion con contrasena incorrecta.
    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowUnauthorized()
    {
        // Arrange
        var request = new LoginRequestDto { Login = "admin@uam.edu.co", Password = "wrong-password" };
        var user = BuildActiveUser();
        _userRepoMock.Setup(r => r.GetByEmailOrUsernameAsync(request.Login)).ReturnsAsync(user);
        _hasherMock.Setup(h => h.Verify(request.Password, user.PasswordHash)).Returns(false);

        // Act
        Func<Task> act = async () => await _service.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
                 .WithMessage("*Credenciales incorrectas*");
        _jwtMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    // Error 2: No se permite iniciar sesion con correo / usuario no registrado.
    [Fact]
    public async Task LoginAsync_WithUnregisteredLogin_ShouldThrowUnauthorized()
    {
        // Arrange
        var request = new LoginRequestDto { Login = "nonexistent@uam.edu.co", Password = "Password123" };
        _userRepoMock.Setup(r => r.GetByEmailOrUsernameAsync(request.Login)).ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _service.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
                 .WithMessage("*Credenciales incorrectas*");
        _hasherMock.Verify(h => h.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _jwtMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    // Error 3: No se permite iniciar sesion con campos vacios.
    [Fact]
    public async Task LoginAsync_WithEmptyLogin_ShouldThrowUnauthorized()
    {
        // Arrange
        var request = new LoginRequestDto { Login = string.Empty, Password = string.Empty };
        _userRepoMock.Setup(r => r.GetByEmailOrUsernameAsync(string.Empty)).ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _service.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        _jwtMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    // Error 4: El servicio propaga errores del repositorio al controller.
    [Fact]
    public async Task LoginAsync_WhenRepositoryThrows_ShouldPropagateError()
    {
        // Arrange
        var request = BuildValidRequest();
        _userRepoMock.Setup(r => r.GetByEmailOrUsernameAsync(It.IsAny<string>()))
                     .ThrowsAsync(new InvalidOperationException("DB connection error"));

        // Act
        Func<Task> act = async () => await _service.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    // HELPERS
    private static LoginRequestDto BuildValidRequest()
    {
        return new LoginRequestDto { Login = "admin@uam.edu.co", Password = "Password123" };
    }

    private static User BuildActiveUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FullName = "Test User",
            Email = "admin@uam.edu.co",
            Username = "admin",
            PasswordHash = "hashed-stored-password",
            IsActive = true,
            Role = new Role { Id = Guid.NewGuid(), Name = "Coordinador" }
        };
    }

    private void SetupSuccessfulLogin(User user)
    {
        _userRepoMock.Setup(r => r.GetByEmailOrUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);
        _hasherMock.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        _jwtMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("fake-jwt-token");
        _jwtMock.Setup(j => j.GetExpiration()).Returns(DateTime.UtcNow.AddMinutes(60));
    }
}
