namespace Gateway.DTOs;

public class InferenceResultDto
{
    public string RequestId { get; set; } = string.Empty;
    public double Entropy { get; set; }
    public double Eccentricity { get; set; }
    public double Confidence { get; set; }
    public bool Flagged { get; set; }
    public double StandardDeviation { get; set; }
    public double MeanAbsoluteDeviation { get; set; }
    public double PeakToAverageRatio { get; set; }
    
    public InferenceResultDto(string requestId, double entropy, double eccentricity, double confidence, bool flagged, double stdDev, double mad, double par)
    {
        RequestId = requestId;
        Entropy = entropy;
        Eccentricity = eccentricity;
        Confidence = confidence;
        Flagged = flagged;
        StandardDeviation = stdDev;
        MeanAbsoluteDeviation = mad;
        PeakToAverageRatio = par;
    }
}