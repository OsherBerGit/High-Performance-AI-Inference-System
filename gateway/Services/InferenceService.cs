using Computation;
using Gateway.Data;
using Gateway.DTOs;
using gateway.Hubs;
using Gateway.Models;
using gateway.Services.Interfaces;
using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace gateway.Services;

public class InferenceService(AppDbContext _dbContext, IConfiguration _configuration, IHubContext<InferenceHub> _hubContext) : IInferenceService
{
    public async Task<InferenceResultDto?> AnalyzeAsync(int userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.Baseline)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return null;
        
        var engineUrl = _configuration["EngineServiceUrl"] ?? "http://localhost:50051";
        using var channel = GrpcChannel.ForAddress(engineUrl);
        var client = new EngineService.EngineServiceClient(channel);

        InferenceResultDto? lastResult = null;

        for (int i = 0; i < 30; i++)
        {
            var request = new FeatureRequest 
            {
                RequestId = Guid.NewGuid().ToString(),
                RawData = Google.Protobuf.ByteString.CopyFrom(user.Baseline?.RawBaseline ?? Array.Empty<byte>())
            };
        
            var reply = await client.ComputeFeaturesAsync(request);
        
            var auditLog = new AuditLog
            {
                RequestId = reply.RequestId,
                UserId = user.Id,
                ShannonEntropy = reply.ShannonEntropy,
                Eccentricity = reply.Eccentricity,
                ConfidenceScore = reply.ConfidenceScore,
                StandardDeviation = reply.StandardDeviation,
                MeanAbsoluteDeviation = reply.MeanAbsoluteDeviation,
                PeakToAverageRatio = reply.PeakToAverageRatio,
                Timestamp = DateTime.UtcNow,
                IsFlagged = reply.ConfidenceScore < 0.9 
            };

            _dbContext.AuditLogs.Add(auditLog);
            await _dbContext.SaveChangesAsync();
        
            lastResult = new InferenceResultDto(
                reply.RequestId, 
                reply.ShannonEntropy, 
                reply.Eccentricity, 
                reply.ConfidenceScore, 
                auditLog.IsFlagged,
                reply.StandardDeviation,
                reply.MeanAbsoluteDeviation,
                reply.PeakToAverageRatio
            );
        
            await _hubContext.Clients.All.SendAsync("ReceiveInferenceUpdate", lastResult);
            await Task.Delay(1000);
        }
    
        return lastResult;
    }
}