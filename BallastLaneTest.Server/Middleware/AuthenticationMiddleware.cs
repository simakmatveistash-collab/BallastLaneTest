using System.Text;

namespace BallastLaneTest.Server.Middleware;

/// <summary>
/// Middleware for handling authentication and extracting user ID from token
/// </summary>
public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                // Decode token (simple base64 format: "userId:email:timestamp")
                var decodedBytes = Convert.FromBase64String(token);
                var decodedString = Encoding.UTF8.GetString(decodedBytes);
                var parts = decodedString.Split(':');

                if (parts.Length >= 2 && int.TryParse(parts[0], out var userId))
                {
                    context.Items["UserId"] = userId;
                    context.Items["UserEmail"] = parts[1];
                }
            }
            catch
            {
                // Invalid token format, skip
            }
        }

        // Check for userId in query parameters (for testing)
        if (context.Request.Query.TryGetValue("userId", out var userIdQuery) && 
            int.TryParse(userIdQuery, out var id))
        {
            context.Items["UserId"] = id;
        }

        await _next(context);
    }
}
