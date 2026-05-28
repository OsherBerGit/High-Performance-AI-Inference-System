using gateway.Services;
using gateway.Services.Interfaces;

namespace Gateway.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IInferenceService, InferenceService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IBaselineService, BaselineService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();

        return services;
    }
}