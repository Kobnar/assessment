using System.ComponentModel.DataAnnotations;

namespace Assessment.Schema;

public class SetPasswordRequestSchema
{
    public required string OldPassword { get; set; }
    
    [MinLength(6)]
    public required string NewPassword { get; set; }
}