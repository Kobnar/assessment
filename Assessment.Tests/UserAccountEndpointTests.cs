using System.Net;
using System.Text;
using Assessment.Models;
using Assessment.Schema;
using Assessment.Services;
using Microsoft.AspNetCore.Authentication;

namespace Assessment.Tests;

[TestFixture]
public class UserAccountEndpointTests : EndpointTestFixture
{
    
    private IAuthenticationService _authenticationService;
    private IAccountsService _accountsService;
    
    [SetUp]
    public new void SetUp()
    {
        _authenticationService = GetService<IAuthenticationService>();
        _accountsService = GetService<IAccountsService>();
        DropCollection("Accounts");
    }

    [Test]
    public async Task SignUp_WithValidCredentials_ReturnsCreated()
    {
        // Compile login request
        var requestBody = Serialize(new SignUpRequestSchema()
            { Username = "test_user", Email = "test@email.com", Password = "test_password" });
        var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
        
        // Submit login request
        var timestamp = DateTime.Now;
        var response = await Client.PostAsync("/account", requestContent);

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
        var text = "{\"username\":\"test_user\",\"email\":\"test@email.com\",\"password\":\"short\"}";
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
        var text = "{\"username\":\"Test_User\",\"email\":\"different@email.com\",\"password\":\"test_password\"}";
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
    public async Task ModifyAccount_WithValidJwt_ReturnsOk()
    {
        string newUsername = "modified_username";
        string mewEmail = "modified@email.com";
        
        // Create existing user
        Account existingAccount = Account.NewAccount("test_user", "test@email.com", "test_password");
        await _accountsService.CreateAsync(existingAccount);

        // Compile request
        string jwt = AuthService.GenerateToken(existingAccount);
        var body = "{\"username\":\"" + newUsername + "\",\"email\":\"" + mewEmail + "\"}";
        var request = new HttpRequestMessage(HttpMethod.Patch, "/account");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        request.Content = new StringContent(body, Encoding.UTF8, "application/json");;

        // Submit request
        var response = await Client.SendAsync(request);

        // Assert expectations about response
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            // TODO: Validate response data
        });
        
        // Query account details
        Account? accountRecord = await _accountsService.GetByUsernameAsync(newUsername);
        Assert.That(accountRecord, Is.Not.Null);
        Assert.That(accountRecord?.Username, Is.EqualTo(newUsername));
        Assert.That(accountRecord?.Email, Is.EqualTo(mewEmail));
    }

    [Test]
    public async Task ModifyAccount_WithNewEmail_UpdatesProfile()
    {
        // Need to grab the profiles service for this specific test, because it needs to check if the profile for
        // an account was updated
        var profilesService = GetService<IProfilesService>();
        
        string oldUsername = "test_user";
        string newUsername = "modified_username";
        string mewEmail = "modified@email.com";
        
        // Create existing user account and grab its ID
        Account? account = Account.NewAccount("test_user", "test@email.com", "test_password");
        await _accountsService.CreateAsync(account);
        account = await _accountsService.GetByUsernameAsync(oldUsername);
        if (account is null)
            Assert.Fail("Failed to create account");
        
        // Create existing profile
        Profile? profile = Profile.NewProfile(
            "test@email.com",
            new Name() { First = "John", Last = "Dowe" },
            "1234567890",
            new Address() { Line1 = "line1", City = "city", Country = "country", State = "ST", PostalCode = "12345" },
            account.Id);
        await profilesService.CreateAsync(profile);
        profile = await profilesService.GetByIdAsync(account.Id);
        if (profile is null)
            Assert.Fail("Failed to create profile");

        // Compile request
        string jwt = AuthService.GenerateToken(account);
        var body = "{\"email\":\"" + mewEmail + "\"}";
        var request = new HttpRequestMessage(HttpMethod.Patch, "/account");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        request.Content = new StringContent(body, Encoding.UTF8, "application/json");;

        // Submit request
        await Client.SendAsync(request);

        // Query profile details
        Profile? profileRecord = await profilesService.GetByIdAsync(account.Id);
        Assert.That(profileRecord, Is.Not.Null);
        Assert.That(profileRecord?.Email, Is.EqualTo(mewEmail));
    }
}