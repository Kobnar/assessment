using System.ComponentModel.DataAnnotations;

namespace Assessment.Schema;

public class QueryAccountsRequestSchema
{
    public int Limit { get; set; } = 100;

    public int Skip { get; set; } = 0;
    
    public string? Username { get; set; }
    
    public string? Email { get; set; }
    
    public DateTime? CreatedAfter { get; set; }
    
    public DateTime? CreatedBefore { get; set; }
}