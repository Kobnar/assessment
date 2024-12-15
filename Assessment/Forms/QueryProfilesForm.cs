using System.ComponentModel.DataAnnotations;

namespace Assessment.Forms;

public class QueryProfilesForm
{
    public int Limit { get; set; } = 100;

    public int Skip { get; set; } = 0;
    
    public string? Name { get; set; }
    
    public string? Email { get; set; }
    
    public string? City { get; set; }
    
    public string? State { get; set; }
    
    public string? PostalCode { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public DateTime? CreatedAfter { get; set; }
    
    public DateTime? CreatedBefore { get; set; }
}