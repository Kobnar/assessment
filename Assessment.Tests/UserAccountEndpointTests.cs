using System.Net;
using System.Text;
using Assessment.Models;
using Assessment.Services;

namespace Assessment.Tests;

[TestFixture]
public class UserAccountEndpointTests : EndpointTestFixture
{
    
    private AccountsService _accountsService;
    
    [SetUp]
    public void Setup()
    {
        _accountsService = GetService<AccountsService>();
        DropCollection("Accounts"); // TODO: Use settings for string
    }

    [Test]
    public async Task SignUp_WithValidCredentials_CreatesUserAccount()
    {
        // Compile login request
        var text = "{\"username\":\"test_user\",\"email\":\"test@email.com\",\"password\":\"test_password\"}";
        var content = new StringContent(text, Encoding.UTF8, "application/json");
        
        // Submit login request
        var timestamp = DateTime.Now;
        var response = await Client.PostAsync("/account", content);

        // Assert expectations about response
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            // TODO: Validate response data
        });
        
        // Query account details
        Account? accountRecord = await _accountsService.GetByUsernameAsync("test_user");
        
        // Assert expectations about side effects
        Assert.Multiple(() =>
        {
            Assert.That(accountRecord, Is.Not.Null);
            Assert.That(accountRecord.Created, Is.GreaterThan(timestamp));
            Assert.That(accountRecord.LastLogin, Is.Null);
        });
    }

    [Test]
    public async Task SignUp_WithShortUsername_ReturnsBadRequest()
    {
        // Compile login request
        var text = "{\"username\":\"abc\",\"email\":\"test@email.com\",\"password\":\"test_password\"}";
        var content = new StringContent(text, Encoding.UTF8, "application/json");
        
        // Submit login request
        var response = await Client.PostAsync("/account", content);

        // Assert expectations about response
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            // TODO: Validate response data
        });
        
        // Query account details
        Account? accountRecord = await _accountsService.GetByUsernameAsync("test_user");
        
        // Assert expectations about side effects
        Assert.That(accountRecord, Is.Null);
    }

    [Test]
    public async Task SignUp_WithShortPassword_ReturnsBadRequest()
    {
        // Compile login request
        var text = "{\"username\":\"test_user\",\"email\":\"test@email.com\",\"password\":\"abcdefghijk\"}";
        var content = new StringContent(text, Encoding.UTF8, "application/json");
        
        // Submit login request
        var response = await Client.PostAsync("/account", content);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            // TODO: Validate response data
        });
        
        // Query account details
        Account? accountRecord = await _accountsService.GetByUsernameAsync("test_user");
        
        // Assert expectations about side effects
        Assert.That(accountRecord, Is.Null);
    }

    [Test]
    public async Task SignUp_WithInvalidEmail_ReturnsBadRequest()
    {
        // Compile login request
        var text = "{\"username\":\"test_user\",\"email\":\"invalid_email\",\"password\":\"test_password\"}";
        var content = new StringContent(text, Encoding.UTF8, "application/json");
        
        // Submit login request
        var response = await Client.PostAsync("/account", content);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            // TODO: Validate response data
        });
        
        // Query account details
        Account? accountRecord = await _accountsService.GetByUsernameAsync("test_user");
        
        // Assert expectations about side effects
        Assert.That(accountRecord, Is.Null);
    }

    [Test]
    public async Task SignUp_WithExistingUsername_ReturnsConflict()
    {
        // Create new account
        Account newAccount = Account.NewAccount("test_user", "test@email.com", "test_password");
        await _accountsService.CreateAsync(newAccount);
        
        // Compile login request
        var text = "{\"username\":\"test_user\",\"email\":\"different@email.com\",\"password\":\"test_password\"}";
        var content = new StringContent(text, Encoding.UTF8, "application/json");
        
        // Submit login request
        var response = await Client.PostAsync("/account", content);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            // TODO: Validate response data
        });
    }

    [Test]
    public async Task SignUp_WithExistingEmail_ReturnsConflict()
    {
        // Create new account
        Account newAccount = Account.NewAccount("test_user", "test@email.com", "test_password");
        await _accountsService.CreateAsync(newAccount);
        
        // Compile login request
        var text = "{\"username\":\"different_user\",\"email\":\"test@email.com\",\"password\":\"test_password\"}";
        var content = new StringContent(text, Encoding.UTF8, "application/json");
        
        // Submit login request
        var response = await Client.PostAsync("/account", content);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            // TODO: Validate response data
        });
    }
}