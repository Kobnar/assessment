using System.ComponentModel.DataAnnotations;

namespace Assessment.Forms;

public class LogInForm
{
    [MinLength(4)]
    public required string Username { get; set; }
    
    [MinLength(12)]
    public required string Password { get; set; }
}