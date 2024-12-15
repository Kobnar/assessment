using System.ComponentModel.DataAnnotations;

namespace Assessment.Forms;

public class SetPasswordForm
{
    public required string OldPassword { get; set; }
    
    [MinLength(6)]
    public required string NewPassword { get; set; }
}