using System.ComponentModel.DataAnnotations;

namespace Assessment.Forms;

public class UpdateProfileForm
{
    public UpdateNameForm? Name { get; set; }
    
    [MinLength(10)]
    public string? PhoneNumber { get; set; }
    
    public UpdateAddressForm? Address { get; set; }
}