using System.Text;
using Assessment.Models;
using Assessment.Services;

namespace Assessment.Tests;

[TestFixture]
public class UserTokenEndpointTests : EndpointTestFixture
{
    private IAccountsService _accountsService;
    
    [SetUp]
    public void Setup()
    {
        _accountsService = GetService<IAccountsService>();
        DropCollection("Accounts");
    }

    [Test]
    public async Task LogIn_WithValidCredentials_UpdatesLastLoggedInAndReturnsJwt()
    {
        // Create new account
        Account newAccount = Account.NewAccount("test_user", "test@email.com", "test_password");
        await _accountsService.CreateAsync(newAccount);
        
        // Compile login request
        var text = "{\"username\":\"test_user\",\"password\":\"test_password\"}";
        var content = new StringContent(text, Encoding.UTF8, "application/json");
        
        // Submit login request
        var timestamp = DateTime.Now;
        var response = await Client.PostAsync("/account/token", content);
        
        // Assert expectations about response
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            // TODO: Validate JWT claims, etc.
        });
        
        // Refresh account details
        Account? accountRecord = await _accountsService.GetByUsernameAsync("test_user");
        
        // Assert expectations about side effects
        Assert.Multiple(() =>
        {
            Assert.That(accountRecord, Is.Not.Null);
            Assert.That(accountRecord.Created, Is.GreaterThan(timestamp));
            Assert.That(accountRecord.LastLogin, Is.GreaterThan(accountRecord.Created));
        });
    }

    [Test]
    public async Task LogIn_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Create new account
        Account account = Account.NewAccount("test_user", "test@email.com", "test_password");
        await _accountsService.CreateAsync(account);
        
        // Compile login request
        var text = "{\"username\":\"test_user\",\"password\":\"wrong_password\"}";
        var content = new StringContent(text, Encoding.UTF8, "application/json");
        
        // Submit login request
        var response = await Client.PostAsync("/account/token", content);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task LogIn_WithUnknownUsername_ReturnsUnauthorized()
    {
        // Create new account
        Account account = Account.NewAccount("test_user", "test@email.com", "test_password");
        await _accountsService.CreateAsync(account);
        
        // Compile login request
        var text = "{\"username\":\"unknown_user\",\"password\":\"test_password\"}";
        var content = new StringContent(text, Encoding.UTF8, "application/json");
        
        // Submit login request
        var response = await Client.PostAsync("/account/token", content);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Unauthorized));
    }
}