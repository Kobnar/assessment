using System.ComponentModel.DataAnnotations;

namespace Assessment.Forms;

public class UpdateUserForm
{
    [MinLength(4)]
    public string? Username { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
}