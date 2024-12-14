using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Assessment.Models;

public class Address
{
    [BsonElement("line1")]
    public string Line1 { get; set; }
    
    [BsonElement("line2")]
    public string? Line2 { get; set; }
    
    [BsonElement("city")]
    public string City { get; set; }
    
    [BsonElement("state")]
    public string State { get; set; }
    
    [BsonElement("country")]
    public string Country { get; set; }
    
    [BsonElement("postalCode")]
    public string PostalCode { get; set; }
}