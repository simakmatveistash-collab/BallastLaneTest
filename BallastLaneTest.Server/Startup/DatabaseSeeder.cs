using BallastLaneTest.Domain.Entities;
using BallastLaneTest.Infrastructure.Data;
using BallastLaneTest.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BallastLaneTest.Server.Startup;

/// <summary>
/// Initializes the database with seed data for development/testing
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        // Check if data already exists
        if (dbContext.Users.Any())
        {
            return;
        }

        // Create test users
        var testUser1 = new User
        {
            Name = "John Doe",
            Email = "john@example.com",
            PasswordHash = passwordHasher.HashPassword("Password123!"),
            CreatedDate = DateTime.UtcNow
        };

        var testUser2 = new User
        {
            Name = "Jane Smith",
            Email = "jane@example.com",
            PasswordHash = passwordHasher.HashPassword("SecurePass456!"),
            CreatedDate = DateTime.UtcNow
        };

        dbContext.Users.Add(testUser1);
        dbContext.Users.Add(testUser2);
        await dbContext.SaveChangesAsync();

        // Create test records
        var record1 = new Record
        {
            Title = "First Task",
            Content = "This is my first task to complete",
            UserId = testUser1.Id,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        var record2 = new Record
        {
            Title = "Meeting Notes",
            Content = "Notes from the team meeting",
            UserId = testUser1.Id,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        var record3 = new Record
        {
            Title = "Project Update",
            Content = "Status update on current project",
            UserId = testUser2.Id,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        dbContext.Records.AddRange(record1, record2, record3);
        await dbContext.SaveChangesAsync();
    }
}
