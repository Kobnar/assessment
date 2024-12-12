using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Assessment.Models;

public class AUser
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("username")]
    public required string Username { get; set; }
    
    [BsonElement("groups")]
    public required List<string> Groups { get; set; } = ["users"];
    
    [BsonElement("created_at")]
    public required DateTime CreatedAt { get; set; }
    
    [BsonElement("password_salt")]
    private string? PasswordSalt { get; set; }
    
    [BsonElement("password_hash")]
    private string? PasswordHash { get; set; }

    public static AUser NewUser(string username, string password)
    {
        AUser newAUser = new()
        {
            Username = username,
            Groups = ["users"],
            CreatedAt = DateTime.Now
        };
        newAUser.SetPassword(password);
        return newAUser;
    }

    public void SetPassword(string newPassword)
    {
        PasswordSalt = GenerateSalt();
        PasswordHash = HashPassword(newPassword, PasswordSalt);
    }

    public bool VerifyPassword(string password)
    {
        if (PasswordHash is null || PasswordSalt is null)
            return false;
        
        return PasswordHash == HashPassword(password, PasswordSalt);
    }

    private static string HashPassword(string password, string salt)
    {
        var saltedPassword = $"{password}{salt}";
        
        // Hash the salted password
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private static string GenerateSalt(int length = 16)
    {
        var newSaltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(newSaltBytes);
            return Convert.ToBase64String(newSaltBytes);
        }
    }
}