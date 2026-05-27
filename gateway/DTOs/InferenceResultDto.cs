namespace Gateway.DTOs;

public class InferenceResultDto
{
    public string RequestId { get; set; } = string.Empty;
    public double Entropy { get; set; }
    public double Eccentricity { get; set; }
    public double Confidence { get; set; }
    public bool Flagged { get; set; }
    
    public InferenceResultDto(string requestId, double entropy, double eccentricity, double confidence, bool flagged)
    {
        RequestId = requestId;
        Entropy = entropy;
        Eccentricity = eccentricity;
        Confidence = confidence;
        Flagged = flagged;
    }
}