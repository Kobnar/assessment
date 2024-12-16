using System.ComponentModel.DataAnnotations;

namespace Assessment.Schema;

public class LogInRequestSchema
{
    [MinLength(4)]
    public required string Username { get; set; }
    
    [MinLength(6)]
    public required string Password { get; set; }
}