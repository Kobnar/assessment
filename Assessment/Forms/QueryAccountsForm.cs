using System.ComponentModel.DataAnnotations;

namespace Assessment.Forms;

public class QueryAccountsForm
{
    public string? Username { get; set; }
    
    public string? Email { get; set; }
    
    public DateTime? CreatedAfter { get; set; }
    
    public DateTime? CreatedBefore { get; set; }
}