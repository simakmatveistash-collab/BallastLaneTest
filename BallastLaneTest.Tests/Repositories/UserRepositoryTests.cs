using BallastLaneTest.Domain.Entities;
using BallastLaneTest.Infrastructure.Repositories;
using BallastLaneTest.Tests.Fixtures;
using NUnit.Framework;

namespace BallastLaneTest.Tests.Repositories;

/// <summary>
/// Unit tests for UserRepository
/// </summary>
public class UserRepositoryTests
{
    private DatabaseFixture _fixture = null!;

    [SetUp]
    public void Setup()
    {
        _fixture = new DatabaseFixture();
    }

    [TearDown]
    public void TearDown()
    {
        _fixture?.Dispose();
    }

    [Test]
    public async Task AddAsync_Should_Create_New_User()
    {
        // Arrange
        var repository = new UserRepository(_fixture.Context);
        var user = new User
        {
            Name = "John Doe",
            Email = "john@example.com",
            PasswordHash = "hashedpassword"
        };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.Name, Is.EqualTo("John Doe"));
    }

    [Test]
    public async Task GetByIdAsync_Should_Return_User_When_Exists()
    {
        // Arrange
        var repository = new UserRepository(_fixture.Context);
        var user = new User
        {
            Name = "Jane Doe",
            Email = "jane@example.com",
            PasswordHash = "hashedpassword"
        };
        var createdUser = await repository.AddAsync(user);

        // Act
        var result = await repository.GetByIdAsync(createdUser.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(createdUser.Id));
        Assert.That(result.Name, Is.EqualTo("Jane Doe"));
    }

    [Test]
    public async Task GetByIdAsync_Should_Return_Null_When_NotExists()
    {
        // Arrange
        var repository = new UserRepository(_fixture.Context);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByEmailAsync_Should_Return_User_When_Exists()
    {
        // Arrange
        var repository = new UserRepository(_fixture.Context);
        var testEmail = "test@example.com";
        var user = new User
        {
            Name = "Test User",
            Email = testEmail,
            PasswordHash = "hashedpassword"
        };
        await repository.AddAsync(user);

        // Act
        var result = await repository.GetByEmailAsync(testEmail);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(testEmail));
    }

    [Test]
    public async Task ExistsByEmailAsync_Should_Return_True_When_User_Exists()
    {
        // Arrange
        var repository = new UserRepository(_fixture.Context);
        var testEmail = "exists@example.com";
        var user = new User
        {
            Name = "Exists User",
            Email = testEmail,
            PasswordHash = "hashedpassword"
        };
        await repository.AddAsync(user);

        // Act
        var result = await repository.ExistsByEmailAsync(testEmail);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsByEmailAsync_Should_Return_False_When_User_Does_Not_Exist()
    {
        // Arrange
        var repository = new UserRepository(_fixture.Context);

        // Act
        var result = await repository.ExistsByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteAsync_Should_Remove_User()
    {
        // Arrange
        var repository = new UserRepository(_fixture.Context);
        var user = new User
        {
            Name = "Delete User",
            Email = "delete@example.com",
            PasswordHash = "hashedpassword"
        };
        var createdUser = await repository.AddAsync(user);

        // Act
        var deleteResult = await repository.DeleteAsync(createdUser.Id);
        var getResult = await repository.GetByIdAsync(createdUser.Id);

        // Assert
        Assert.That(deleteResult, Is.True);
        Assert.That(getResult, Is.Null);
    }
}
