using BallastLaneTest.Application.Dtos;
using BallastLaneTest.Domain.Entities;
using BallastLaneTest.Domain.Repositories;

namespace BallastLaneTest.Application.Services.Implementations;

/// <summary>
/// Implementation of authentication service
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> RegisterUserAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException($"User with email '{request.Email}' already exists.");
        }

        // Create new user
        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash,
            CreatedDate = DateTime.UtcNow
        };

        var createdUser = await _userRepository.AddAsync(user, cancellationToken);

        return MapToUserDto(createdUser);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException($"User with email '{request.Email}' not found.");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid password.");
        }

        // Generate token
        var token = GenerateToken(user.Id, user.Email);

        return new LoginResponse
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Token = token
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user is null ? null : MapToUserDto(user);
    }

    public string GenerateToken(int userId, string email)
    {
        // Simple token generation using userId and email
        // In production, use JWT or other secure token mechanism
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{userId}:{email}:{DateTime.UtcNow.Ticks}"));
    }

    private static UserDto MapToUserDto(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        CreatedDate = user.CreatedDate
    };
}
