using Gateway.Data;
using Gateway.Models;
using Gateway.Endpoints;
using Gateway.Extensions;
using gateway.Hubs;
using Microsoft.EntityFrameworkCore;
using Gateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

SwaggerExtensions.AddSwaggerDocumentation(builder.Services);
IdentityServiceExtensions.AddIdentityServices(builder.Services, builder.Configuration);
RateLimitingExtensions.AddRateLimiterConfig(builder.Services);

ServiceExtensions.AddApplicationServices(builder.Services);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddMemoryCache();

var app = builder.Build();

Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate(); 
    
    if (!context.Users.Any())
    {
        var testUser = new User
        {
            Username = "osher_analyst",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Role = "Analyst"
        };
        context.Users.Add(testUser); 
        context.SaveChanges();
        
        context.UserBaselines.Add(new UserBaseline { UserId = testUser.Id, RawBaseline = Enumerable.Repeat((byte)255, 100).ToArray() });
        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseRateLimiter();

app.UseCors("ReactPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapInferenceEndpoints();
app.MapAuditEndpoints();
app.MapBaselineEndpoints();
app.MapHub<InferenceHub>("/hubs/inference");

app.Run();