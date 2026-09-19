namespace BallastLaneTest.Application.Dtos;

/// <summary>
/// DTO for create/update record request
/// </summary>
public class CreateRecordRequest
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}
