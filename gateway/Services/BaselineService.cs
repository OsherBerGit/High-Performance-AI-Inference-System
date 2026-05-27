using Gateway.Data;
using Gateway.Models;
using gateway.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gateway.Services;

public class BaselineService(AppDbContext _context) : IBaselineService
{
    public async Task<bool> UpdateBaselineAsync(int userId, byte[] newBaseline)
    {
        var user = await _context.Users
            .Include(u => u.Baseline)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user is null) return false;
        
        if (user.Baseline is null)
        {
            var userBaseline = new UserBaseline
            {
                RawBaseline = newBaseline,
                UserId = user.Id
            };
                
            user.Baseline = userBaseline;
        }
        else
            user.Baseline.RawBaseline = newBaseline;

        await _context.SaveChangesAsync();
        
        return true;
    }
}