using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gateway.Models;

public class AuditLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string RequestId { get; set; } = string.Empty;

    public int UserId { get; set; }
    public User? User { get; set; }

    public double ShannonEntropy { get; set; }
    public double Eccentricity { get; set; }
    public double ConfidenceScore { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsFlagged { get; set; } = false;
}