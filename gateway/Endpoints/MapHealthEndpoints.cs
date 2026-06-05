using Computation; 

namespace Gateway.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/health/engine", async (IConfiguration configuration) =>
        {
            try
            {
                var engineUrl = configuration["EngineServiceUrl"] ?? "http://localhost:50051";
                using var channel = Grpc.Net.Client.GrpcChannel.ForAddress(engineUrl);
                var client = new EngineService.EngineServiceClient(channel);
                
                await client.ComputeFeaturesAsync(new FeatureRequest 
                { 
                    RequestId = "health-check", 
                    RawData = Google.Protobuf.ByteString.Empty 
                });
                
                return Results.Ok(new { status = "Healthy", engine = "Connected" });
            }
            catch
            {
                return Results.Json(new { status = "Unhealthy", engine = "Disconnected" }, statusCode: 503);
            }
        });
    }
}