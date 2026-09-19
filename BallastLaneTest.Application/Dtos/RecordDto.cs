namespace BallastLaneTest.Application.Dtos;

/// <summary>
/// DTO for record data transfer
/// </summary>
public class RecordDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
