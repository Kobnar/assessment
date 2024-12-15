using System.ComponentModel.DataAnnotations;
using Assessment.Models;

namespace Assessment.Forms;

public class SignUpForm
{
    [MinLength(4)]
    public required string Username { get; set; }
    
    [EmailAddress]
    public required string Email { get; set; }
    
    [MinLength(6)]
    public required string Password { get; set; }
}