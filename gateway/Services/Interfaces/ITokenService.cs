using Gateway.Models;

namespace gateway.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}