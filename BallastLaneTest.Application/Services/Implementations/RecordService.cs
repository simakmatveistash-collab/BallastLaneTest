using BallastLaneTest.Application.Dtos;
using BallastLaneTest.Domain.Entities;
using BallastLaneTest.Domain.Repositories;

namespace BallastLaneTest.Application.Services.Implementations;

/// <summary>
/// Implementation of record service for CRUD operations
/// </summary>
public class RecordService : IRecordService
{
    private readonly IRecordRepository _recordRepository;

    public RecordService(IRecordRepository recordRepository)
    {
        _recordRepository = recordRepository;
    }

    public async Task<IEnumerable<RecordDto>> GetUserRecordsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var records = await _recordRepository.GetByUserIdAsync(userId, cancellationToken);
        return records.Select(MapToRecordDto);
    }

    public async Task<RecordDto?> GetRecordByIdAsync(int recordId, CancellationToken cancellationToken = default)
    {
        var record = await _recordRepository.GetByIdAsync(recordId, cancellationToken);
        return record is null ? null : MapToRecordDto(record);
    }

    public async Task<RecordDto> CreateRecordAsync(int userId, CreateRecordRequest request, CancellationToken cancellationToken = default)
    {
        var record = new Record
        {
            Title = request.Title,
            Content = request.Content,
            UserId = userId,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        var createdRecord = await _recordRepository.AddAsync(record, cancellationToken);
        return MapToRecordDto(createdRecord);
    }

    public async Task<RecordDto> UpdateRecordAsync(int recordId, int userId, UpdateRecordRequest request, CancellationToken cancellationToken = default)
    {
        // Verify ownership
        var isOwner = await _recordRepository.ExistsByIdAndUserIdAsync(recordId, userId, cancellationToken);
        if (!isOwner)
        {
            throw new InvalidOperationException("User does not have permission to update this record.");
        }

        var record = await _recordRepository.GetByIdAsync(recordId, cancellationToken);
        if (record is null)
        {
            throw new InvalidOperationException($"Record with ID {recordId} not found.");
        }

        record.Title = request.Title;
        record.Content = request.Content;
        record.UpdatedDate = DateTime.UtcNow;

        var updatedRecord = await _recordRepository.UpdateAsync(record, cancellationToken);
        return MapToRecordDto(updatedRecord);
    }

    public async Task<bool> DeleteRecordAsync(int recordId, int userId, CancellationToken cancellationToken = default)
    {
        // Verify ownership
        var isOwner = await _recordRepository.ExistsByIdAndUserIdAsync(recordId, userId, cancellationToken);
        if (!isOwner)
        {
            throw new InvalidOperationException("User does not have permission to delete this record.");
        }

        return await _recordRepository.DeleteAsync(recordId, cancellationToken);
    }

    public async Task<bool> IsRecordOwnerAsync(int recordId, int userId, CancellationToken cancellationToken = default)
    {
        return await _recordRepository.ExistsByIdAndUserIdAsync(recordId, userId, cancellationToken);
    }

    private static RecordDto MapToRecordDto(Record record) => new()
    {
        Id = record.Id,
        Title = record.Title,
        Content = record.Content,
        UserId = record.UserId,
        CreatedDate = record.CreatedDate,
        UpdatedDate = record.UpdatedDate
    };
}
