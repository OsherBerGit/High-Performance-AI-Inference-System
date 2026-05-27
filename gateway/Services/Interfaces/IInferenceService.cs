using Gateway.DTOs;

namespace gateway.Services.Interfaces;

public interface IInferenceService
{
    Task<InferenceResultDto?> AnalyzeAsync(int userId);
}