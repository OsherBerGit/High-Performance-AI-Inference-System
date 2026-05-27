using System.Security.Claims;
using Gateway.Extensions;
using gateway.Services.Interfaces;

namespace Gateway.Endpoints;

public static class InferenceEndpoints
{
    public static void MapInferenceEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/analyze", async (ClaimsPrincipal user, IInferenceService inferenceService) =>
        {
            var userId = user.GetUserId();
            var result = await inferenceService.AnalyzeAsync(userId);
            
            if (result is null) 
                return Results.Unauthorized();

            return Results.Ok(result);
        })
        .RequireAuthorization()
        .RequireRateLimiting("fixed_policy");
    }
}