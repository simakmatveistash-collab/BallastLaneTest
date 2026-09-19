using Microsoft.EntityFrameworkCore;
using BallastLaneTest.Domain.Entities;
using BallastLaneTest.Domain.Repositories;
using BallastLaneTest.Infrastructure.Data;

namespace BallastLaneTest.Infrastructure.Repositories;

/// <summary>
/// Record repository implementation using Entity Framework Core
/// </summary>
public class RecordRepository : IRecordRepository
{
    private readonly ApplicationDbContext _context;

    public RecordRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Record?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Records.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Record>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Records
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Record>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Records.ToListAsync(cancellationToken);
    }

    public async Task<Record> AddAsync(Record record, CancellationToken cancellationToken = default)
    {
        _context.Records.Add(record);
        await _context.SaveChangesAsync(cancellationToken);
        return record;
    }

    public async Task<Record> UpdateAsync(Record record, CancellationToken cancellationToken = default)
    {
        _context.Records.Update(record);
        await _context.SaveChangesAsync(cancellationToken);
        return record;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var record = await GetByIdAsync(id, cancellationToken);
        if (record is null)
            return false;

        _context.Records.Remove(record);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExistsByIdAndUserIdAsync(int recordId, int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Records
            .AnyAsync(r => r.Id == recordId && r.UserId == userId, cancellationToken);
    }
}
