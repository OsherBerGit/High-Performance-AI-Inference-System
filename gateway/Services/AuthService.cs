using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gateway.Data;
using Gateway.DTOs;
using gateway.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace gateway.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly string _jwtSecret;
    
    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _jwtSecret = configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT Secret not configured.");
    }
    
    public async Task<string?> LoginAsync(UserLoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
        
        if (user is null || loginDto.Password != "password123") return null;

        var key = Encoding.ASCII.GetBytes(_jwtSecret);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { 
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            }, "Jwt"),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}