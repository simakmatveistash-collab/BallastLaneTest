using BallastLaneTest.Application.Dtos;

namespace BallastLaneTest.Application.Services;

/// <summary>
/// Service interface for record CRUD operations
/// </summary>
public interface IRecordService
{
    /// <summary>
    /// Get all records for a user
    /// </summary>
    Task<IEnumerable<RecordDto>> GetUserRecordsAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a single record by ID
    /// </summary>
    Task<RecordDto?> GetRecordByIdAsync(int recordId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new record
    /// </summary>
    Task<RecordDto> CreateRecordAsync(int userId, CreateRecordRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing record
    /// </summary>
    Task<RecordDto> UpdateRecordAsync(int recordId, int userId, UpdateRecordRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a record
    /// </summary>
    Task<bool> DeleteRecordAsync(int recordId, int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify if a record belongs to a user
    /// </summary>
    Task<bool> IsRecordOwnerAsync(int recordId, int userId, CancellationToken cancellationToken = default);
}
