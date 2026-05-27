namespace gateway.Services.Interfaces;

public interface IBaselineService
{
    Task<bool> UpdateBaselineAsync(int userId, byte[] newBaseline);
}