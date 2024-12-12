using MongoDB.Bson.Serialization.Attributes;

namespace Assessment.Models;

public enum PhoneNumberType
{
    Mobile,
    Home,
    Work,
}

public class PhoneNumber
{
    [BsonElement]
    public required PhoneNumberType Type { get; set; }
    
    [BsonElement]
    public required string CountryCode { get; set; } = "+1";
    
    [BsonElement]
    public required string Number { get; set; }
}