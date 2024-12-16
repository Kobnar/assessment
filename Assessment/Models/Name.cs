using MongoDB.Bson.Serialization.Attributes;

namespace Assessment.Models;

public class Name
{
    [BsonElement("first")]
    public required string First { get; set; }

    [BsonElement("middle")]
    public string? Middle { get; set; }
    
    [BsonElement("last")]
    public required string Last { get; set; }

    public string Full => Middle == null ? $"{First} {Last}" : $"{First} {Middle} {Last}";
}