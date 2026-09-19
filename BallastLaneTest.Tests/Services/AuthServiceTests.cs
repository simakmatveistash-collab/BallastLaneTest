using BallastLaneTest.Application.Dtos;
using BallastLaneTest.Application.Services;
using BallastLaneTest.Application.Services.Implementations;
using BallastLaneTest.Domain.Entities;
using BallastLaneTest.Domain.Repositories;
using Moq;
using NUnit.Framework;

namespace BallastLaneTest.Tests.Services;

/// <summary>
/// Unit tests for AuthService
/// </summary>
public class AuthServiceTests
{
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<IPasswordHasher> _mockPasswordHasher = null!;
    private AuthService _service = null!;

    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _service = new AuthService(_mockUserRepository.Object, _mockPasswordHasher.Object);
    }

    [Test]
    public async Task RegisterUserAsync_Should_Create_New_User()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Name = "John Doe",
            Email = "john@example.com",
            Password = "password123"
        };

        _mockUserRepository.Setup(r => r.ExistsByEmailAsync(request.Email, default))
            .ReturnsAsync(false);

        var hashedPassword = "hashed_password";
        _mockPasswordHasher.Setup(h => h.HashPassword(request.Password))
            .Returns(hashedPassword);

        var createdUser = new User
        {
            Id = 1,
            Name = request.Name,
            Email = request.Email,
            PasswordHash = hashedPassword
        };

        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), default))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _service.RegisterUserAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("John Doe"));
        Assert.That(result.Email, Is.EqualTo("john@example.com"));
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), default), Times.Once);
    }

    [Test]
    public void RegisterUserAsync_Should_Throw_When_User_Exists()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Name = "John Doe",
            Email = "john@example.com",
            Password = "password123"
        };

        _mockUserRepository.Setup(r => r.ExistsByEmailAsync(request.Email, default))
            .ReturnsAsync(true);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.RegisterUserAsync(request));
    }

    [Test]
    public async Task LoginAsync_Should_Return_LoginResponse_When_Credentials_Valid()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "john@example.com",
            Password = "password123"
        };

        var user = new User
        {
            Id = 1,
            Name = "John Doe",
            Email = request.Email,
            PasswordHash = "hashed_password"
        };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email, default))
            .ReturnsAsync(user);

        _mockPasswordHasher.Setup(h => h.VerifyPassword(request.Password, user.PasswordHash))
            .Returns(true);

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserId, Is.EqualTo(user.Id));
        Assert.That(result.Name, Is.EqualTo("John Doe"));
        Assert.That(result.Email, Is.EqualTo("john@example.com"));
        Assert.That(result.Token, Is.Not.Empty);
    }

    [Test]
    public void LoginAsync_Should_Throw_When_User_Not_Found()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email, default))
            .ReturnsAsync((User?)null);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.LoginAsync(request));
    }

    [Test]
    public void LoginAsync_Should_Throw_When_Password_Invalid()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "john@example.com",
            Password = "wrongpassword"
        };

        var user = new User
        {
            Id = 1,
            Name = "John Doe",
            Email = request.Email,
            PasswordHash = "hashed_password"
        };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email, default))
            .ReturnsAsync(user);

        _mockPasswordHasher.Setup(h => h.VerifyPassword(request.Password, user.PasswordHash))
            .Returns(false);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.LoginAsync(request));
    }
}
