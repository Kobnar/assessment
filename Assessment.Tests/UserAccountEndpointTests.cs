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
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        
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

        // Assert expectations about the response
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        
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

        // Assert expectations about the response
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        
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

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
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

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [Test]
    public async Task ViewAccount_WithValidJwt_ReturnsAccount()
    {
        // Create new account
        Account newAccount = Account.NewAccount("test_user", "test@email.com", "test_password");
        await _accountsService.CreateAsync(newAccount);
        Account? accountRecord = await _accountsService.GetByUsernameAsync("test_user");
        
        // Compile request
        string jwt = AuthService.GenerateToken(accountRecord);
        var body = "{\"username\":\"different_user\",\"email\":\"test@email.com\",\"password\":\"test_password\"}";
        var request = new HttpRequestMessage(HttpMethod.Get, "/account");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        request.Content = new StringContent(body, Encoding.UTF8, "application/json");;
        
        // Submit request
        var response = await Client.SendAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            // TODO: Validate response data
        });
    }

    [Test]
    public async Task ViewAccount_WithInvalidJwt_ReturnsUnauthorized()
    {
        // Create new account
        Account newAccount = Account.NewAccount("test_user", "test@email.com", "test_password");
        await _accountsService.CreateAsync(newAccount);
        Account? accountRecord = await _accountsService.GetByUsernameAsync("test_user");
        
        // Compile request
        var body = "{\"username\":\"different_user\",\"email\":\"test@email.com\",\"password\":\"test_password\"}";
        var request = new HttpRequestMessage(HttpMethod.Get, "/account");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalid_jwt");
        request.Content = new StringContent(body, Encoding.UTF8, "application/json");;
        
        // Submit request
        var response = await Client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task ViewAccount_WithExpiredJwt_ReturnsUnauthorized()
    {
        Assert.Fail("Need to figure out how to change exp claim");
    }

    [Test]
    public async Task ViewAccount_WithManipulatedJwt_ReturnsUnauthorized()
    {
        Assert.Fail("Need to modify flip bit in signature");
    }
}