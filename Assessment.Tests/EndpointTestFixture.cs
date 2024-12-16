using System.Text.Json;
using Assessment.Services;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Assessment.Tests;

/// <summary>
/// Encapsulates the common tasks of setting up an application factory and provides an
/// interface to grab specific services.
/// </summary>
public class EndpointTestFixture
{
    private JsonSerializerOptions _jsonOptions;
    private CustomWebApplicationFactory<Program> _factory;
    private IMongoDatabase _database;
    protected HttpClient Client;
    protected IAuthService AuthService;
    
    protected T GetService<T>() where T : notnull
    {
        return _factory.GetService<T>();
    }

    protected string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj, _jsonOptions);
    }

    protected T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    protected IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }

    protected void DropCollection(string name)
    {
        _database.DropCollection(name);
    }
    
    [SetUp]
    public void SetUp()
    {
        // Create the web app factory and client
        _factory = new CustomWebApplicationFactory<Program>();
        Client = _factory.CreateClient(); // Create an HTTP client
        
        // Get low-level MongoDB database connection
        var dbName = _factory.GetConfiguration()["Database:DatabaseName"];
        var mongoClient = _factory.Services.GetRequiredService<IMongoClient>();
        _database = mongoClient.GetDatabase(dbName);
        
        // Configure JSON serialization/deserialization
        _jsonOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        
        // Extract authentication resource
        AuthService = GetService<IAuthService>();
    }

    [TearDown]
    public void TearDown()
    {
        Client.Dispose();
        _factory.Dispose();
    }
}