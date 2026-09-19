namespace BallastLaneTest.Application.Dtos;

/// <summary>
/// DTO for login response containing user info
/// </summary>
public class LoginResponse
{
    public int UserId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Token { get; set; }
}
