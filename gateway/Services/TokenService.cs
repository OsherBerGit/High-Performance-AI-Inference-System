using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gateway.Models;
using gateway.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace gateway.Services;

public class TokenService(IConfiguration _configuration) : ITokenService
{
    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(user.Role.Split(',').Select(role => new Claim(ClaimTypes.Role, role.Trim())));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetSection("JwtSettings:SecretKey").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration.GetSection("JwtSettings:Issuer").Value,
            audience: _configuration.GetSection("JwtSettings:Audience").Value,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}