using System.ComponentModel.DataAnnotations;
using Assessment.Models;

namespace Assessment.Forms;

public class UpdateProfileForm
{
    public Name? Name { get; set; }
    
    [MinLength(10)]
    public string? PhoneNumber { get; set; }
    
    public Address? Address { get; set; }
}