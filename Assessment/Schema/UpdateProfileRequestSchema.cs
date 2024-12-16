using System.ComponentModel.DataAnnotations;

namespace Assessment.Schema;

public class UpdateProfileRequestSchema
{
    public UpdateNameRequestSchema? Name { get; set; }
    
    [MinLength(10)]
    public string? PhoneNumber { get; set; }
    
    public UpdateAddressRequestSchema? Address { get; set; }
}