using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Assessment.Models;
using Assessment.Schema;
using Assessment.Services;

namespace Assessment.Tests;

[TestFixture]
public class UserTokenEndpointTests : EndpointTestFixture
{
    private JsonSerializerOptions _options;
    private IAuthService _authService;
    private IAccountsService _accountsService;

    private string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj, _options);
    }

    private T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, _options);
    }
    
    [SetUp]
    public void SetUp()
    {
        _options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        _authService = GetService<IAuthService>();
        _accountsService = GetService<IAccountsService>();
        DropCollection("Accounts");
    }

    [Test]
    public async Task LogIn_WithValidCredentials_ReturnsOk()
    {
        // Create new account
        var account = Account.NewAccount("test_user", "test@email.com", "test_password");
        await _accountsService.CreateAsync(account);
        
        // Compile login request
        var requestBody = Serialize(new LogInRequestSchema() {Username = "test_user", Password = "test_password"});
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        
        // Submit login request
        var timestamp = DateTime.Now;
        var response = await Client.PostAsync("/account/token", content);
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        
        // Deserialize JWT
        var responseBody = await response.Content.ReadAsStringAsync();
        var responseData = Deserialize<LogInResponseSchema>(responseBody);
        Assert.That(responseData, Is.Not.Null);
        
        // Validate JWT
        var principal = _authService.ValidateToken(responseData.Token);
        Assert.That(principal, Is.Not.Null);
        
        // Verify updated account login timestamp
        var accountRecord = await _accountsService.GetByUsernameAsync("test_user");
        Assert.That(accountRecord, Is.Not.Null);
        Assert.That(accountRecord.LastLogin, Is.GreaterThan(timestamp));
    }

    [Test]
    public async Task LogIn_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Create new account
        var account = Account.NewAccount("test_user", "test@email.com", "test_password");
        await _accountsService.CreateAsync(account);
        
        // Compile login request
        var requestBody = Serialize(new LogInRequestSchema() {Username = "test_user", Password = "wrong_password"});
        var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
        
        // Submit login request
        var response = await Client.PostAsync("/account/token", requestContent);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task LogIn_WithUnknownUsername_ReturnsUnauthorized()
    {
        // Create new account
        Account account = Account.NewAccount("test_user", "test@email.com", "test_password");
        await _accountsService.CreateAsync(account);
        
        // Compile login request
        var requestBody = Serialize(new LogInRequestSchema() {Username = "unknown_user", Password = "test_password"});
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        
        // Submit login request
        var response = await Client.PostAsync("/account/token", content);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Unauthorized));
    }
}