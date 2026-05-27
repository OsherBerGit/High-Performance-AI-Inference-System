using gateway.Services.Interfaces;

namespace Gateway.Endpoints;

public static class AuditEndpoints
{
    public static void MapAuditEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/audit/history", async (IAuditService auditService) =>
        {
            var result = await auditService.GetHistoryReportAsync();
            return Results.Ok(result);
        })
        .RequireAuthorization();
    }
}