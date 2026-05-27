using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Gateway.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddRateLimiterConfig(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddFixedWindowLimiter("fixed_policy", limiterOptions =>
            {
                limiterOptions.PermitLimit = 5;
                limiterOptions.Window = TimeSpan.FromSeconds(10);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 0;
            });
        });

        return services;
    }
}