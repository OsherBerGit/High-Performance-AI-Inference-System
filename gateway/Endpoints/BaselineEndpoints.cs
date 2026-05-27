using System.Security.Claims;
using Gateway.DTOs;
using Gateway.Extensions;
using gateway.Services.Interfaces;

namespace Gateway.Endpoints;

public static class BaselineEndpoints
{
    public static void MapBaselineEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/baseline/upload", async (UploadBaselineDto dto, ClaimsPrincipal user, IBaselineService baselineService) =>
        {
            var userId = user.GetUserId();
            
            var isUpdated = await baselineService.UpdateBaselineAsync(userId, dto.NewBaseline);
            
            if (!isUpdated) 
                return Results.NotFound("User not found.");
            
            return Results.Ok(new { Message = "Baseline updated successfully." });
        })
        .RequireAuthorization()
        .RequireRateLimiting("fixed_policy");
    }
}