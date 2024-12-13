using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Assessment.Tests;

[TestFixture]
public class AdminAccountsEndpointTests
{
    private CustomWebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private AuthService _authService;
    private IMongoDatabase _database;
    private AccountsService _accountsService;
    private IMongoCollection<Account> _accountsCollection;
    
    [SetUp]
    public void Setup()
    {
        // Create the web app factory and client
        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.CreateClient(); // Create an HTTP client
        
        // Extract authentication resource
        _authService = _factory.GetService<AuthService>();
        
        // Get MongoDB database connection
        var dbName = _factory.GetConfiguration()["Database:DatabaseName"];
        var mongoClient = _factory.Services.GetRequiredService<IMongoClient>();
        _database = mongoClient.GetDatabase(dbName);

        // Drop the collection to ensure a clean state for each test
        _database.DropCollection("Accounts");
        _accountsCollection = _database.GetCollection<Account>("Accounts");
        
        // Extract model resources
        _accountsService = _factory.GetService<AccountsService>();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task LogIn_WithValidCredentials_ReturnsCreated()
    {
        // Create new account
        Account account = Account.NewAccount("test_user", "test@email.com", "test_password");
        await _accountsService.CreateAsync(account);
        
        // Submit login request
        var text = "{\"username\":\"test_user\",\"password\":\"test_password\"}";
        var content = new StringContent(text, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/account/token", content);
        var jwt = await response.Content.ReadAsStringAsync();
        var isValidJwt = _authService.ValidateToken("cats");
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Created));
        Assert.That(jwt is not null);
    }
    
    [Test]
    public async Task SignUp_CreatesAccount()
    {
        var url = "/accounts";
        var text = "{\"username\":\"test_user\",\"password\":\"test_password\"}";
        var content = new StringContent(text, Encoding.UTF8, "application/json");

        var timestamp = DateTime.Now;
        var response = await _client.PostAsync(url, content);
        var dbRecord = await _accountsService.GetByUsernameAsync("test_user");

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Created));
        Assert.That(dbRecord, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dbRecord.Created, Is.GreaterThan(timestamp));
            Assert.That(dbRecord.VerifyPassword("test_password"));
        });
    }

    [Test]
    public async Task SignUp_RejectsDuplicateUsername()
    {
        var url = "/accounts";
        var text = "{\"username\":\"test_user\",\"password\":\"test_password\"}";
        var content = new StringContent(text, Encoding.UTF8, "application/json");
        
        await _client.PostAsync(url, content);
        var response = await _client.PostAsync(url, content);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Conflict));
    }
}