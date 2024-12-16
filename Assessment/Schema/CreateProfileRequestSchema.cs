using System.ComponentModel.DataAnnotations;
using Assessment.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Assessment.Schema;

public class CreateProfileRequestSchema
{
    public required Name Name { get; set; }
    
    [MinLength(10)]
    public required string PhoneNumber { get; set; }
    
    public required Address Address { get; set; }
}