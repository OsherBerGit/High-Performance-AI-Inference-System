using gateway.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace gateway.Services;

public class TokenBlacklistService(IMemoryCache _cache) : ITokenBlacklistService
{
    public Task BlacklistTokenAsync(string token, DateTime expiry) 
    {
        _cache.Set(token, true, expiry - DateTime.UtcNow);
        return Task.CompletedTask;
    }

    public Task<bool> IsTokenBlacklistedAsync(string token) 
    {
        return Task.FromResult(_cache.TryGetValue(token, out _));
    }
}