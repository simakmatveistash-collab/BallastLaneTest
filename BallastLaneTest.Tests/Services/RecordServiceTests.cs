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
    [Test]
    public async Task CreateRecordAsync_Should_Create_New_Record()
    {
        // Arrange
        var mockRecordRepository = new Mock<IRecordRepository>();
        var request = new CreateRecordRequest
        {
            Title = "Test Record",
            Content = "Test Content"
        };

        var createdRecord = new Record
        {
            Id = 1,
            Title = request.Title,
            Content = request.Content,
            UserId = 1
        };

        mockRecordRepository.Setup(r => r.AddAsync(It.IsAny<Record>(), default))
            .ReturnsAsync(createdRecord);

        var service = new RecordService(mockRecordRepository.Object);

        // Act
        var result = await service.CreateRecordAsync(1, request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo("Test Record"));
        Assert.That(result.Content, Is.EqualTo("Test Content"));
        mockRecordRepository.Verify(r => r.AddAsync(It.IsAny<Record>(), default), Times.Once);
    }

    [Test]
    public async Task GetUserRecordsAsync_Should_Return_User_Records()
    {
        // Arrange
        var mockRecordRepository = new Mock<IRecordRepository>();
        var userId = 1;

        var records = new List<Record>
        {
            new Record { Id = 1, Title = "Record 1", Content = "Content 1", UserId = userId },
            new Record { Id = 2, Title = "Record 2", Content = "Content 2", UserId = userId }
        };

        mockRecordRepository.Setup(r => r.GetByUserIdAsync(userId, default))
            .ReturnsAsync(records);

        var service = new RecordService(mockRecordRepository.Object);

        // Act
        var result = await service.GetUserRecordsAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        var recordList = result.ToList();
        Assert.That(recordList.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetRecordByIdAsync_Should_Return_Record()
    {
        // Arrange
        var mockRecordRepository = new Mock<IRecordRepository>();
        var record = new Record
        {
            Id = 1,
            Title = "Test Record",
            Content = "Test Content",
            UserId = 1
        };

        mockRecordRepository.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(record);

        var service = new RecordService(mockRecordRepository.Object);

        // Act
        var result = await service.GetRecordByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo("Test Record"));
    }

    [Test]
    public async Task UpdateRecordAsync_Should_Update_Record_When_Owner()
    {
        // Arrange
        var mockRecordRepository = new Mock<IRecordRepository>();
        var userId = 1;
        var recordId = 1;
        var request = new UpdateRecordRequest
        {
            Title = "Updated Title",
            Content = "Updated Content"
        };

        var record = new Record
        {
            Id = recordId,
            Title = "Original Title",
            Content = "Original Content",
            UserId = userId
        };

        mockRecordRepository.Setup(r => r.ExistsByIdAndUserIdAsync(recordId, userId, default))
            .ReturnsAsync(true);
        mockRecordRepository.Setup(r => r.GetByIdAsync(recordId, default))
            .ReturnsAsync(record);
        mockRecordRepository.Setup(r => r.UpdateAsync(It.IsAny<Record>(), default))
            .ReturnsAsync(record);

        var service = new RecordService(mockRecordRepository.Object);

        // Act
        var result = await service.UpdateRecordAsync(recordId, userId, request);

        // Assert
        Assert.That(result, Is.Not.Null);
        mockRecordRepository.Verify(r => r.UpdateAsync(It.IsAny<Record>(), default), Times.Once);
    }

    [Test]
    public async Task UpdateRecordAsync_Should_Throw_When_Not_Owner()
    {
        // Arrange
        var mockRecordRepository = new Mock<IRecordRepository>();
        var userId = 1;
        var recordId = 1;
        var request = new UpdateRecordRequest
        {
            Title = "Updated Title",
            Content = "Updated Content"
        };

        mockRecordRepository.Setup(r => r.ExistsByIdAndUserIdAsync(recordId, userId, default))
            .ReturnsAsync(false);

        var service = new RecordService(mockRecordRepository.Object);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.UpdateRecordAsync(recordId, userId, request));
    }

    [Test]
    public async Task DeleteRecordAsync_Should_Delete_Record_When_Owner()
    {
        // Arrange
        var mockRecordRepository = new Mock<IRecordRepository>();
        var userId = 1;
        var recordId = 1;

        mockRecordRepository.Setup(r => r.ExistsByIdAndUserIdAsync(recordId, userId, default))
            .ReturnsAsync(true);
        mockRecordRepository.Setup(r => r.DeleteAsync(recordId, default))
            .ReturnsAsync(true);

        var service = new RecordService(mockRecordRepository.Object);

        // Act
        var result = await service.DeleteRecordAsync(recordId, userId);

        // Assert
        Assert.That(result, Is.True);
        mockRecordRepository.Verify(r => r.DeleteAsync(recordId, default), Times.Once);
    }

    [Test]
    public async Task DeleteRecordAsync_Should_Throw_When_Not_Owner()
    {
        // Arrange
        var mockRecordRepository = new Mock<IRecordRepository>();
        var userId = 1;
        var recordId = 1;

        mockRecordRepository.Setup(r => r.ExistsByIdAndUserIdAsync(recordId, userId, default))
            .ReturnsAsync(false);

        var service = new RecordService(mockRecordRepository.Object);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.DeleteRecordAsync(recordId, userId));
    }
}
