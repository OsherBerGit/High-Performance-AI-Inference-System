using Gateway.DTOs;
using gateway.Services.Interfaces;

namespace Gateway.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (UserLoginDto loginDto, IAuthService authService) =>
        {
            var token = await authService.LoginAsync(loginDto);
            return token is null ? Results.Unauthorized() : Results.Ok(new { Token = token });
        });
    }
}