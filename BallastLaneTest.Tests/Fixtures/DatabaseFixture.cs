using Microsoft.EntityFrameworkCore;
using BallastLaneTest.Infrastructure.Data;

namespace BallastLaneTest.Tests.Fixtures;

/// <summary>
/// Test fixture for in-memory database context
/// </summary>
public class DatabaseFixture : IDisposable
{
    private readonly ApplicationDbContext _context;

    public ApplicationDbContext Context => _context;

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
