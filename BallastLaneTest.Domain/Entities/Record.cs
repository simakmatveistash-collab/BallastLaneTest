namespace BallastLaneTest.Domain.Entities;

/// <summary>
/// Record entity representing a user's record/note
/// </summary>
public class Record
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    // Navigation property
    public User? User { get; set; }
}
