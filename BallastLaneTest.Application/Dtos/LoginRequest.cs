namespace BallastLaneTest.Application.Dtos;

/// <summary>
/// DTO for user login request
/// </summary>
public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
