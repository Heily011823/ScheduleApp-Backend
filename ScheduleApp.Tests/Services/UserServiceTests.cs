using FluentAssertions;
using Moq;
using ScheduleApp.Application.DTOs;
using ScheduleApp.Application.Interfaces;
using ScheduleApp.Application.Services;
using ScheduleApp.Domain.Entities;

namespace ScheduleApp.Tests.Services;

// Autor: Jacobo
// Version: 0.2
// Pruebas unitarias del UserService (HU-168).
// Patron AAA (Arrange, Act, Assert) aplicado en cada test.
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPasswordHasher> _hasherMock;
    private readonly UserService _service;

    private static readonly Guid AdministradorRoleId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _hasherMock = new Mock<IPasswordHasher>();
        _hasherMock.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed-password");
        _service = new UserService(_userRepoMock.Object, _hasherMock.Object);
    }

    // CDA 1: Crear usuario con datos validos.
    [Fact]
    public async Task CreateUserAsync_WithValidData_ShouldReturnUserDto()
    {
        // Arrange
        var dto = BuildValidCreateDto();
        _userRepoMock.Setup(r => r.GetByEmailIncludingDeletedAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateUserAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("juan.perez@uam.edu.co");
        result.FullName.Should().Be("Juan Perez");
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    // CDA 2: Consultar listado de usuarios.
    [Fact]
    public async Task GetUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), FullName = "User 1", Email = "u1@test.com" },
            new() { Id = Guid.NewGuid(), FullName = "User 2", Email = "u2@test.com" }
        };
        _userRepoMock.Setup(r => r.SearchUsersAsync(null, null, null)).ReturnsAsync(users);

        // Act
        var result = await _service.GetUsersAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(u => u.Email == "u1@test.com");
    }

    // CDA 3: Obtener usuario por Id.
    [Fact]
    public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnUserDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var role = new Role { Id = AdministradorRoleId, Name = "Administrador" };
        var user = new User
        {
            Id = userId,
            FullName = "Test User",
            Email = "test@uam.edu.co",
            Username = "testuser",
            IdentityDocument = "12345",
            IsActive = true,
            Role = role,
            RoleId = role.Id,
            CreatedAt = DateTime.UtcNow
        };
        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _service.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.Email.Should().Be("test@uam.edu.co");
        result.RoleName.Should().Be("Administrador");
    }

    // CDA 4: Actualizar usuario existente.
    [Fact]
    public async Task UpdateUserAsync_WhenUserExists_ShouldUpdateAndReturnDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new User
        {
            Id = userId,
            FullName = "Old Name",
            Email = "old@test.com",
            Username = "olduser",
            IdentityDocument = "111",
            IsActive = true
        };
        var updateDto = new UpdateUserDto
        {
            FullName = "New Name",
            Email = "new@test.com",
            Username = "newuser",
            IdentityDocument = "222",
            Role = "Coordinador",
            IsActive = true
        };
        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        _userRepoMock.Setup(r => r.GetByEmailIncludingDeletedAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateUserAsync(userId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.FullName.Should().Be("New Name");
        result.Email.Should().Be("new@test.com");
        _userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    // CDA 5: Eliminar usuario (soft delete con IsDeleted).
    [Fact]
    public async Task DeleteUserAsync_WhenUserActive_ShouldMarkAsDeletedAndReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FullName = "Test",
            Email = "test@test.com",
            IsActive = true,
            IsDeleted = false
        };
        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteUserAsync(userId);

        // Assert
        result.Should().BeTrue();
        user.IsDeleted.Should().BeTrue();
        _userRepoMock.Verify(r => r.UpdateAsync(It.Is<User>(u => u.IsDeleted)), Times.Once);
    }

    // CDA 6: Asignar rol Administrador a un usuario.
    [Fact]
    public async Task CreateUserAsync_WithAdministradorRole_ShouldAssignAdminRoleId()
    {
        // Arrange
        var dto = BuildValidCreateDto();
        dto.Role = "Administrador";
        _userRepoMock.Setup(r => r.GetByEmailIncludingDeletedAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        User? capturedUser = null;
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                     .Callback<User>(u => capturedUser = u)
                     .Returns(Task.CompletedTask);

        // Act
        await _service.CreateUserAsync(dto);

        // Assert
        capturedUser.Should().NotBeNull();
        capturedUser!.RoleId.Should().Be(AdministradorRoleId);
    }

    // CDA 7: Busqueda + filtros + paginacion.
    [Fact]
    public async Task GetPagedUsersAsync_WithFilters_ShouldReturnPagedResultWithMetadata()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), FullName = "Admin User", Email = "admin@test.com", IsActive = true }
        };
        _userRepoMock.Setup(r => r.GetPagedAsync("Admin", "Administrador", true, 1, 10))
                     .ReturnsAsync((users, 1));

        // Act
        var result = await _service.GetPagedUsersAsync("Admin", "Administrador", true, 1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }

    // CDA 8: Datos retornados corresponden a la informacion registrada.
    [Fact]
    public async Task GetUsersAsync_ShouldReturnUsersWithCorrectFields()
    {
        // Arrange
        var registeredId = Guid.NewGuid();
        var users = new List<User>
        {
            new()
            {
                Id = registeredId,
                FullName = "Registered User",
                Email = "registered@uam.edu.co",
                Username = "reguser",
                IdentityDocument = "555",
                IsActive = true,
                CreatedAt = new DateTime(2026, 5, 1)
            }
        };
        _userRepoMock.Setup(r => r.SearchUsersAsync(null, null, null)).ReturnsAsync(users);

        // Act
        var result = await _service.GetUsersAsync();

        // Assert
        var first = result.First();
        first.Id.Should().Be(registeredId);
        first.Email.Should().Be("registered@uam.edu.co");
        first.FullName.Should().Be("Registered User");
        first.Username.Should().Be("reguser");
    }

    // Error 1: No se permite crear con correo duplicado.
    [Fact]
    public async Task CreateUserAsync_WithDuplicateEmail_ShouldThrowInvalidOperation()
    {
        // Arrange
        var dto = BuildValidCreateDto();
        var existing = new User { Id = Guid.NewGuid(), Email = dto.Email.ToLower().Trim() };
        _userRepoMock.Setup(r => r.GetByEmailIncludingDeletedAsync(It.IsAny<string>())).ReturnsAsync(existing);

        // Act
        Func<Task> act = async () => await _service.CreateUserAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*Ya existe un usuario*");
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    // Error 2: Si el repositorio lanza, el service lo propaga.
    [Fact]
    public async Task CreateUserAsync_WhenRepositoryThrows_ShouldPropagateError()
    {
        // Arrange
        var dto = BuildValidCreateDto();
        _userRepoMock.Setup(r => r.GetByEmailIncludingDeletedAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                     .ThrowsAsync(new InvalidOperationException("DB constraint violation"));

        // Act
        Func<Task> act = async () => await _service.CreateUserAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    // Error 3: No se permite actualizar a un email ya usado por OTRO usuario.
    [Fact]
    public async Task UpdateUserAsync_WithDuplicateEmail_ShouldThrowInvalidOperation()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new User
        {
            Id = userId,
            FullName = "User A",
            Email = "old@test.com",
            IsActive = true
        };
        var anotherUserWithEmail = new User { Id = Guid.NewGuid(), Email = "duplicate@test.com" };
        var updateDto = new UpdateUserDto
        {
            FullName = "User A",
            Email = "duplicate@test.com",
            Username = "usera",
            IdentityDocument = "123",
            Role = "Coordinador",
            IsActive = true
        };
        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        _userRepoMock.Setup(r => r.GetByEmailIncludingDeletedAsync(It.IsAny<string>())).ReturnsAsync(anotherUserWithEmail);

        // Act
        Func<Task> act = async () => await _service.UpdateUserAsync(userId, updateDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*Ya existe otro usuario*");
    }

    // Error 4: Consulta de usuario inexistente devuelve null.
    [Fact]
    public async Task GetUserByIdAsync_WhenUserNotExists_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetUserByIdAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    // Error 5: Update de usuario inexistente devuelve null y no llama al repo.
    [Fact]
    public async Task UpdateUserAsync_WhenUserNotExists_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateDto = new UpdateUserDto
        {
            FullName = "Whatever",
            Email = "x@test.com",
            Username = "x",
            IdentityDocument = "x",
            Role = "Coordinador",
            IsActive = true
        };
        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await _service.UpdateUserAsync(userId, updateDto);

        // Assert
        result.Should().BeNull();
        _userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    // Error 6: Delete de usuario inexistente devuelve false y no llama al repo.
    [Fact]
    public async Task DeleteUserAsync_WhenUserNotExists_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await _service.DeleteUserAsync(userId);

        // Assert
        result.Should().BeFalse();
        _userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    // HELPERS
    private static CreateUserDto BuildValidCreateDto()
    {
        return new CreateUserDto
        {
            FullName = "Juan Perez",
            Email = "juan.perez@uam.edu.co",
            Username = "jperez",
            IdentityDocument = "1234567890",
            Password = "Password123",
            ConfirmPassword = "Password123",
            Role = "Coordinador",
            Status = "Activo"
        };
    }
}
