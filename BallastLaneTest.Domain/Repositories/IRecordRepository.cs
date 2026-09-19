using BallastLaneTest.Domain.Entities;

namespace BallastLaneTest.Domain.Repositories;

/// <summary>
/// Repository interface for Record entity
/// </summary>
public interface IRecordRepository
{
    /// <summary>
    /// Get record by ID
    /// </summary>
    Task<Record?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all records for a specific user
    /// </summary>
    Task<IEnumerable<Record>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all records
    /// </summary>
    Task<IEnumerable<Record>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a new record
    /// </summary>
    Task<Record> AddAsync(Record record, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing record
    /// </summary>
    Task<Record> UpdateAsync(Record record, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a record by ID
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if record exists and belongs to user
    /// </summary>
    Task<bool> ExistsByIdAndUserIdAsync(int recordId, int userId, CancellationToken cancellationToken = default);
}
