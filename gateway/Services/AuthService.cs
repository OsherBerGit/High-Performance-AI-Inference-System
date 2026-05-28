using Gateway.Data;
using Gateway.DTOs;
using gateway.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gateway.Services;

public class AuthService(AppDbContext _context, ITokenService _tokenService) : IAuthService
{
    public async Task<string?> LoginAsync(UserLoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
        
        if (user is null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");
        
        return _tokenService.GenerateToken(user);
    }
}