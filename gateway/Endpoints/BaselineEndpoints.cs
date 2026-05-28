using System.Security.Claims;
using Gateway.Extensions;
using gateway.Services.Interfaces;

namespace Gateway.Endpoints;

public static class BaselineEndpoints
{
    public static void MapBaselineEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/baseline/upload", async (HttpRequest request, ClaimsPrincipal user, IBaselineService baselineService) =>
        {
            if (!request.HasFormContentType || request.Form.Files.Count == 0)
                return Results.BadRequest("No file uploaded.");
            
            var file = request.Form.Files[0];
            
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();
            
            var userId = user.GetUserId();
            var isUpdated = await baselineService.UpdateBaselineAsync(userId, fileBytes);
            
            if (!isUpdated) 
                return Results.NotFound("User not found.");
            
            return Results.Ok(new { Message = "Baseline updated successfully." });
        })
        .RequireAuthorization()
        .DisableAntiforgery();
    }
}