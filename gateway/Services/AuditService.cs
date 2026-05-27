using Gateway.Data;
using Gateway.DTOs;
using gateway.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gateway.Services;

public class AuditService(AppDbContext _context) : IAuditService
{
    public async Task<List<AuditReportDto>> GetHistoryReportAsync()
    {
        var reportQuery =
            from log in _context.AuditLogs
            join user in _context.Users on log.UserId equals user.Id
            join baseline in _context.UserBaselines on user.Id equals baseline.UserId
            orderby log.Timestamp descending
            select new AuditReportDto(
                log.Id,
                log.Timestamp,
                user.Username,
                user.Role,
                baseline.RawBaseline.Length,
                log.ShannonEntropy,
                log.IsFlagged
            );

        var finalReport = await reportQuery.Take(50).ToListAsync();
        return finalReport;
    }
}