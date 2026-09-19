namespace BallastLaneTest.Application.Dtos;

/// <summary>
/// DTO for update record request
/// </summary>
public class UpdateRecordRequest
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}
