using BallastLaneTest.Domain.Entities;
using BallastLaneTest.Infrastructure.Repositories;
using BallastLaneTest.Tests.Fixtures;
using NUnit.Framework;

namespace BallastLaneTest.Tests.Repositories;

/// <summary>
/// Unit tests for RecordRepository
/// </summary>
public class RecordRepositoryTests
{
    private DatabaseFixture _fixture = null!;
    private RecordRepository _repository = null!;
    private UserRepository _userRepository = null!;

    [SetUp]
    public void Setup()
    {
        _fixture = new DatabaseFixture();
        _repository = new RecordRepository(_fixture.Context);
        _userRepository = new UserRepository(_fixture.Context);
    }

    [TearDown]
    public void TearDown()
    {
        _fixture?.Dispose();
    }

    private async Task<User> CreateTestUser()
    {
        var user = new User
        {
            Name = "Test User",
            Email = $"user{Guid.NewGuid()}@example.com",
            PasswordHash = "hashedpassword"
        };
        return await _userRepository.AddAsync(user);
    }

    [Test]
    public async Task AddAsync_Should_Create_New_Record()
    {
        // Arrange
        var user = await CreateTestUser();
        var record = new Record
        {
            Title = "Test Record",
            Content = "Test Content",
            UserId = user.Id
        };

        // Act
        var result = await _repository.AddAsync(record);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.Title, Is.EqualTo("Test Record"));
    }

    [Test]
    public async Task GetByIdAsync_Should_Return_Record_When_Exists()
    {
        // Arrange
        var user = await CreateTestUser();
        var record = new Record
        {
            Title = "Existing Record",
            Content = "Existing Content",
            UserId = user.Id
        };
        var createdRecord = await _repository.AddAsync(record);

        // Act
        var result = await _repository.GetByIdAsync(createdRecord.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(createdRecord.Id));
        Assert.That(result.Title, Is.EqualTo("Existing Record"));
    }

    [Test]
    public async Task GetByUserIdAsync_Should_Return_Only_User_Records()
    {
        // Arrange
        var user1 = await CreateTestUser();
        var user2 = await CreateTestUser();

        var record1 = new Record { Title = "Record 1", Content = "Content 1", UserId = user1.Id };
        var record2 = new Record { Title = "Record 2", Content = "Content 2", UserId = user1.Id };
        var record3 = new Record { Title = "Record 3", Content = "Content 3", UserId = user2.Id };

        await _repository.AddAsync(record1);
        await _repository.AddAsync(record2);
        await _repository.AddAsync(record3);

        // Act
        var result = await _repository.GetByUserIdAsync(user1.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        var records = result.ToList();
        Assert.That(records.Count, Is.EqualTo(2));
        foreach (var r in records)
        {
            Assert.That(r.UserId, Is.EqualTo(user1.Id));
        }
    }

    [Test]
    public async Task ExistsByIdAndUserIdAsync_Should_Return_True_When_Record_Belongs_To_User()
    {
        // Arrange
        var user = await CreateTestUser();
        var record = new Record
        {
            Title = "User Record",
            Content = "Content",
            UserId = user.Id
        };
        var createdRecord = await _repository.AddAsync(record);

        // Act
        var result = await _repository.ExistsByIdAndUserIdAsync(createdRecord.Id, user.Id);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsByIdAndUserIdAsync_Should_Return_False_When_Record_Does_Not_Belong_To_User()
    {
        // Arrange
        var user1 = await CreateTestUser();
        var user2 = await CreateTestUser();
        var record = new Record
        {
            Title = "User1 Record",
            Content = "Content",
            UserId = user1.Id
        };
        var createdRecord = await _repository.AddAsync(record);

        // Act
        var result = await _repository.ExistsByIdAndUserIdAsync(createdRecord.Id, user2.Id);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateAsync_Should_Modify_Record()
    {
        // Arrange
        var user = await CreateTestUser();
        var record = new Record
        {
            Title = "Original Title",
            Content = "Original Content",
            UserId = user.Id
        };
        var createdRecord = await _repository.AddAsync(record);
        createdRecord.Title = "Updated Title";
        createdRecord.Content = "Updated Content";

        // Act
        var result = await _repository.UpdateAsync(createdRecord);

        // Assert
        Assert.That(result.Title, Is.EqualTo("Updated Title"));
        Assert.That(result.Content, Is.EqualTo("Updated Content"));
    }

    [Test]
    public async Task DeleteAsync_Should_Remove_Record()
    {
        // Arrange
        var user = await CreateTestUser();
        var record = new Record
        {
            Title = "Delete Record",
            Content = "Content",
            UserId = user.Id
        };
        var createdRecord = await _repository.AddAsync(record);

        // Act
        var deleteResult = await _repository.DeleteAsync(createdRecord.Id);
        var getResult = await _repository.GetByIdAsync(createdRecord.Id);

        // Assert
        Assert.That(deleteResult, Is.True);
        Assert.That(getResult, Is.Null);
    }
}
