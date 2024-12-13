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
    private CustomWebApplicationFactory<Program> _factory;
    private IMongoDatabase _database;
    protected HttpClient Client;
    protected AuthService AuthService;
    
    protected T GetService<T>()
    {
        return _factory.GetService<T>();
    }

    protected void DropCollection(string name)
    {
        _database.DropCollection(name);
    }

    protected void GetCollection<T>(string name)
    {
        _database.GetCollection<T>(name);
    }
    
    [SetUp]
    public void Setup()
    {
        // Create the web app factory and client
        _factory = new CustomWebApplicationFactory<Program>();
        Client = _factory.CreateClient(); // Create an HTTP client
        
        // Get low-level MongoDB database connection
        var dbName = _factory.GetConfiguration()["Database:DatabaseName"];
        var mongoClient = _factory.Services.GetRequiredService<IMongoClient>();
        _database = mongoClient.GetDatabase(dbName);
        
        // Extract authentication resource
        AuthService = GetService<AuthService>();
    }

    [TearDown]
    public void TearDown()
    {
        Client.Dispose();
        _factory.Dispose();
    }
}