using Gateway.Data;
using Gateway.Models;
using Microsoft.EntityFrameworkCore;
using Grpc.Net.Client;
using Computation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("HybridPipelineDb"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    context.Database.EnsureCreated();

    if (!context.Users.Any())
    {
        var testUser = new User
        {
            Username = "osher_analyst",
            PasswordHash = "fake_bcrypt_hash_for_now",
            Role = "Analyst"
        };

        context.Users.Add(testUser);
        context.SaveChanges();

        var userBaseline = new UserBaseline
        {
            UserId = testUser.Id,
            RawBaseline = Enumerable.Repeat((byte)255, 100).ToArray() 
        };

        context.UserBaselines.Add(userBaseline);
        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/analyze", async (AppDbContext dbContext) =>
{
    using var channel = GrpcChannel.ForAddress("http://localhost:50051");
    
    var client = new EngineService.EngineServiceClient(channel);

    var user = dbContext.Users
        .Include(u => u.Baseline)
        .FirstOrDefault(u => u.Username == "osher_analyst");

    var request = new FeatureRequest 
    {
        RequestId = Guid.NewGuid().ToString(),
        RawData = Google.Protobuf.ByteString.CopyFrom(user?.Baseline?.RawBaseline ?? Array.Empty<byte>())
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
