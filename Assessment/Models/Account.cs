using System.Security.Cryptography;
using System.Text;
using Assessment.Forms;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Assessment.Models;

public class Account
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    // TODO: Must be unique
    [BsonElement("username")]
    public required string Username { get; set; }
    
    [BsonElement("groups")]
    public required List<string> Groups { get; set; } = ["users"];
    
    [BsonElement("created")]
    public required DateTime Created { get; set; }
    
    [BsonElement("last_login")]
    public DateTime? LastLogin { get; set; }
    
    [BsonElement("password_salt")]
    private string? PasswordSalt { get; set; }
    
    [BsonElement("password_hash")]
    private string? PasswordHash { get; set; }

    public static Account NewAccount(string username, string password)
    {
        // TODO: Create admin account flow (CLI only)
        Account newAccount = new()
        {
            Username = username,
            Groups = ["users"],
            Created = DateTime.Now,
        };
        newAccount.SetPassword(password);
        return newAccount;
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