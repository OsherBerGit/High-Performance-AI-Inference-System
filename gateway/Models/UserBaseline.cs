using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gateway.Models;

public class UserBaseline
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public byte[] RawBaseline { get; set; } = Array.Empty<byte>();
    
    public User? User { get; set; }
}