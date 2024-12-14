using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Assessment.Models;

public class Profile
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("email")]
    public required string Email { get; set; }
    
    [BsonElement("name")]
    public required Name Name { get; set; }
    
    [BsonElement("phoneNumber")]
    public required string PhoneNumber { get; set; }
    
    [BsonElement("address")]
    public required Address Address { get; set; }
    
    [BsonElement("created")]
    public required DateTime Created { get; set; }

    public static Profile NewProfile(string email, Name name, string phoneNumber, Address address, string? userId = null)
    {
        return new()
        {
            Id = userId,
            Email = email,
            Name = name,
            PhoneNumber = phoneNumber,
            Address = address,
            Created = DateTime.Now,
        };
    }
}