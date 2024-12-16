namespace Assessment.Models;

public class QueryResult<T> where T : notnull
{
    public int Limit { get; set; }
    
    public int Skip { get; set; }
    
    public long Count { get; set; }
    
    public List<T>? Items { get; set; }
}