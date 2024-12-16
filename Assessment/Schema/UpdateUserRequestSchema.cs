using System.ComponentModel.DataAnnotations;

namespace Assessment.Schema;

public class UpdateUserRequestSchema
{
    [MinLength(4)]
    public string? Username { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
}