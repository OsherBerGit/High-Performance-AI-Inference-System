using Gateway.DTOs;

namespace gateway.Services.Interfaces;

public interface IAuthService
{
    Task<string?> LoginAsync(UserLoginDto loginDto);
}