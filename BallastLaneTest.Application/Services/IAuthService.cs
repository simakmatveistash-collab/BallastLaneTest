using BallastLaneTest.Application.Dtos;

namespace BallastLaneTest.Application.Services;

/// <summary>
/// Service interface for user authentication and management
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Register a new user
    /// </summary>
    Task<UserDto> RegisterUserAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Login user and return login response with token
    /// </summary>
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user by ID
    /// </summary>
    Task<UserDto?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate JWT token for user
    /// </summary>
    string GenerateToken(int userId, string email);
}
