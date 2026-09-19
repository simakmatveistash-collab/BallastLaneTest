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
    [Test]
    public async Task RegisterUserAsync_Should_Create_New_User()
    {
        // Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        var request = new RegisterUserRequest
        {
            Name = "John Doe",
            Email = "john@example.com",
            Password = "password123"
        };

        mockUserRepository.Setup(r => r.ExistsByEmailAsync(request.Email, default))
            .ReturnsAsync(false);

        var hashedPassword = "hashed_password";
        mockPasswordHasher.Setup(h => h.HashPassword(request.Password))
            .Returns(hashedPassword);

        var createdUser = new User
        {
            Id = 1,
            Name = request.Name,
            Email = request.Email,
            PasswordHash = hashedPassword
        };

        mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), default))
            .ReturnsAsync(createdUser);

        var service = new AuthService(mockUserRepository.Object, mockPasswordHasher.Object);

        // Act
        var result = await service.RegisterUserAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("John Doe"));
        Assert.That(result.Email, Is.EqualTo("john@example.com"));
        mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), default), Times.Once);
    }

    [Test]
    public async Task RegisterUserAsync_Should_Throw_When_User_Exists()
    {
        // Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        var request = new RegisterUserRequest
        {
            Name = "John Doe",
            Email = "john@example.com",
            Password = "password123"
        };

        mockUserRepository.Setup(r => r.ExistsByEmailAsync(request.Email, default))
            .ReturnsAsync(true);

        var service = new AuthService(mockUserRepository.Object, mockPasswordHasher.Object);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.RegisterUserAsync(request));
    }

    [Test]
    public async Task LoginAsync_Should_Return_LoginResponse_When_Credentials_Valid()
    {
        // Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

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

        mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email, default))
            .ReturnsAsync(user);

        mockPasswordHasher.Setup(h => h.VerifyPassword(request.Password, user.PasswordHash))
            .Returns(true);

        var service = new AuthService(mockUserRepository.Object, mockPasswordHasher.Object);

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserId, Is.EqualTo(user.Id));
        Assert.That(result.Name, Is.EqualTo("John Doe"));
        Assert.That(result.Email, Is.EqualTo("john@example.com"));
        Assert.That(result.Token, Is.Not.Empty);
    }

    [Test]
    public async Task LoginAsync_Should_Throw_When_User_Not_Found()
    {
        // Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };

        mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email, default))
            .ReturnsAsync((User?)null);

        var service = new AuthService(mockUserRepository.Object, mockPasswordHasher.Object);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.LoginAsync(request));
    }

    [Test]
    public async Task LoginAsync_Should_Throw_When_Password_Invalid()
    {
        // Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

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

        mockUserRepository.Setup(r => r.GetByEmailAsync(request.Email, default))
            .ReturnsAsync(user);

        mockPasswordHasher.Setup(h => h.VerifyPassword(request.Password, user.PasswordHash))
            .Returns(false);

        var service = new AuthService(mockUserRepository.Object, mockPasswordHasher.Object);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.LoginAsync(request));
    }
}
