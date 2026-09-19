using BallastLaneTest.Application.Dtos;
using BallastLaneTest.Application.Services.Implementations;
using BallastLaneTest.Domain.Entities;
using BallastLaneTest.Domain.Repositories;
using Moq;
using NUnit.Framework;

namespace BallastLaneTest.Tests.Services;

/// <summary>
/// Unit tests for RecordService
/// </summary>
public class RecordServiceTests
{
    private const int UserId = 1;
    private const int RecordId = 1;

    private Mock<IRecordRepository> _mockRecordRepository = null!;
    private RecordService _service = null!;

    [SetUp]
    public void Setup()
    {
        _mockRecordRepository = new Mock<IRecordRepository>();
        _service = new RecordService(_mockRecordRepository.Object);
    }

    [Test]
    public async Task CreateRecordAsync_Should_Create_New_Record()
    {
        // Arrange
        var request = new CreateRecordRequest
        {
            Title = "Test Record",
            Content = "Test Content"
        };

        var createdRecord = new Record
        {
            Id = RecordId,
            Title = request.Title,
            Content = request.Content,
            UserId = UserId
        };

        _mockRecordRepository.Setup(r => r.AddAsync(It.IsAny<Record>(), default))
            .ReturnsAsync(createdRecord);

        // Act
        var result = await _service.CreateRecordAsync(UserId, request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo("Test Record"));
        Assert.That(result.Content, Is.EqualTo("Test Content"));
        _mockRecordRepository.Verify(r => r.AddAsync(It.IsAny<Record>(), default), Times.Once);
    }

    [Test]
    public async Task GetUserRecordsAsync_Should_Return_User_Records()
    {
        // Arrange
        var records = new List<Record>
        {
            new Record { Id = 1, Title = "Record 1", Content = "Content 1", UserId = UserId },
            new Record { Id = 2, Title = "Record 2", Content = "Content 2", UserId = UserId }
        };

        _mockRecordRepository.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync(records);

        // Act
        var result = await _service.GetUserRecordsAsync(UserId);

        // Assert
        Assert.That(result, Is.Not.Null);
        var recordList = result.ToList();
        Assert.That(recordList.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetRecordByIdAsync_Should_Return_Record()
    {
        // Arrange
        var record = new Record
        {
            Id = RecordId,
            Title = "Test Record",
            Content = "Test Content",
            UserId = UserId
        };

        _mockRecordRepository.Setup(r => r.GetByIdAsync(RecordId, default))
            .ReturnsAsync(record);

        // Act
        var result = await _service.GetRecordByIdAsync(RecordId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo("Test Record"));
    }

    [Test]
    public async Task UpdateRecordAsync_Should_Update_Record_When_Owner()
    {
        // Arrange
        var request = new UpdateRecordRequest
        {
            Title = "Updated Title",
            Content = "Updated Content"
        };

        var record = new Record
        {
            Id = RecordId,
            Title = "Original Title",
            Content = "Original Content",
            UserId = UserId
        };

        _mockRecordRepository.Setup(r => r.ExistsByIdAndUserIdAsync(RecordId, UserId, default))
            .ReturnsAsync(true);
        _mockRecordRepository.Setup(r => r.GetByIdAsync(RecordId, default))
            .ReturnsAsync(record);
        _mockRecordRepository.Setup(r => r.UpdateAsync(It.IsAny<Record>(), default))
            .ReturnsAsync(record);

        // Act
        var result = await _service.UpdateRecordAsync(RecordId, UserId, request);

        // Assert
        Assert.That(result, Is.Not.Null);
        _mockRecordRepository.Verify(r => r.UpdateAsync(It.IsAny<Record>(), default), Times.Once);
    }

    [Test]
    public void UpdateRecordAsync_Should_Throw_When_Not_Owner()
    {
        // Arrange
        var request = new UpdateRecordRequest
        {
            Title = "Updated Title",
            Content = "Updated Content"
        };

        _mockRecordRepository.Setup(r => r.ExistsByIdAndUserIdAsync(RecordId, UserId, default))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.UpdateRecordAsync(RecordId, UserId, request));
    }

    [Test]
    public async Task DeleteRecordAsync_Should_Delete_Record_When_Owner()
    {
        // Arrange
        _mockRecordRepository.Setup(r => r.ExistsByIdAndUserIdAsync(RecordId, UserId, default))
            .ReturnsAsync(true);
        _mockRecordRepository.Setup(r => r.DeleteAsync(RecordId, default))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteRecordAsync(RecordId, UserId);

        // Assert
        Assert.That(result, Is.True);
        _mockRecordRepository.Verify(r => r.DeleteAsync(RecordId, default), Times.Once);
    }

    [Test]
    public void DeleteRecordAsync_Should_Throw_When_Not_Owner()
    {
        // Arrange
        _mockRecordRepository.Setup(r => r.ExistsByIdAndUserIdAsync(RecordId, UserId, default))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.DeleteRecordAsync(RecordId, UserId));
    }
}
