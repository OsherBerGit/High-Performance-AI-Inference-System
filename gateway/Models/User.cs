using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gateway.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Analyst";

    public List<AuditLog> AuditLogs { get; set; } = new();
    public UserBaseline? Baseline { get; set; }
}