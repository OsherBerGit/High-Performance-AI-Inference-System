using System.Security.Claims;
using Gateway.Extensions;
using gateway.Services.Interfaces;

namespace Gateway.Endpoints;

public static class InferenceEndpoints
{
    public static void MapInferenceEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/inference/analyze", async (ClaimsPrincipal user, IInferenceService inferenceService) =>
        {
            var userId = user.GetUserId();
            var result = await inferenceService.AnalyzeAsync(userId);
            return result is null ? Results.Unauthorized() : Results.Ok(result);
        })
        .RequireAuthorization()
        .RequireRateLimiting("fixed_policy");
    }
}