using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Assessment.Models;

public class Profile
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }
    
    [BsonElement("email")]
    public required string Email { get; set; }
    
    [BsonElement("name")]
    public required Name Name { get; set; }
    
    [BsonElement("phone")]
    public required PhoneNumber PhoneNumber { get; set; }
    
    [BsonElement("address")]
    public required Address Address { get; set; }
}