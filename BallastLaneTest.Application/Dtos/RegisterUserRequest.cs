namespace BallastLaneTest.Application.Dtos;

/// <summary>
/// DTO for user registration request
/// </summary>
public class RegisterUserRequest
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
