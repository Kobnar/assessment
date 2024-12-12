using MongoDB.Bson.Serialization.Attributes;

namespace Assessment.Models;

public class Address
{
    [BsonElement]
    public string Line1 { get; set; }
    
    [BsonElement]
    public string Line2 { get; set; }
    
    [BsonElement]
    public string City { get; set; }
    
    [BsonElement]
    public string State { get; set; }
    
    [BsonElement]
    public string Country { get; set; }
    
    [BsonElement]
    public string PostalCode { get; set; }
}