using Gateway.DTOs;

namespace gateway.Services.Interfaces;

public interface IAuditService
{
    Task<List<AuditReportDto>> GetHistoryReportAsync();
}