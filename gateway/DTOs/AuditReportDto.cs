namespace Gateway.DTOs;

public class AuditReportDto
{
    public int LogId { get; set; }
    public DateTime AnalysisTime { get; set; }
    public string AnalystName { get; set;} = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int BaselineSize { get; set; }
    public double ResultEntropy { get; set; }
    public bool IsAnomaly { get; set; }
    
    public AuditReportDto(int logId, DateTime analysisTime, string analystName, string role, int baselineSize, double resultEntropy, bool isAnomaly)
    {
        LogId = logId;
        AnalysisTime = analysisTime;
        AnalystName = analystName;
        Role = role;
        BaselineSize = baselineSize;
        ResultEntropy = resultEntropy;
        IsAnomaly = isAnomaly;
    }
}