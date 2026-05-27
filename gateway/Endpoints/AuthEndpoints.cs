using Gateway.DTOs;
using gateway.Services.Interfaces;

namespace Gateway.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/login", async (UserLoginDto loginDto, IAuthService authService) =>
        {
            var token = await authService.LoginAsync(loginDto);
            
            if (token is null)
                return Results.Unauthorized();
                
            return Results.Ok(new { Token = token });
        });
    }
}