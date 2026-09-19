namespace BallastLaneTest.Application.Dtos;

/// <summary>
/// DTO for user data transfer
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedDate { get; set; }
}
