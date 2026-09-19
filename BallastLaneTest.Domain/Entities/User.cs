namespace BallastLaneTest.Domain.Entities;

/// <summary>
/// User entity representing a user in the system
/// </summary>
public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<Record> Records { get; set; } = [];
}
