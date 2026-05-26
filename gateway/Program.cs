using Grpc.Net.Client;
using Computation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/analyze", async () =>
{
    using var channel = GrpcChannel.ForAddress("http://localhost:50051");
    
    var client = new EngineService.EngineServiceClient(channel);

    var request = new FeatureRequest 
    {
        RequestId = Guid.NewGuid().ToString(),
        RawData = Google.Protobuf.ByteString.CopyFrom(new byte[100])
    };

    try
    {
        var reply = await client.ComputeFeaturesAsync(request);
        
        return Results.Ok(new 
        {
            RequestId = reply.RequestId,
            Entropy = reply.ShannonEntropy,
            Eccentricity = reply.Eccentricity,
            Confidence = reply.ConfidenceScore
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Rust Engine Error: {ex.Message}");
    }
});

app.Run();
