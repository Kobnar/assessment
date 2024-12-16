using Assessment.Controllers;
using Assessment.Schema;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Assessment.Tests;

[TestFixture]
public class UserTokenControllerTests : ControllerTestFixture
{
    private UserTokenController _controller;

    [SetUp]
    public new void SetUp()
    {
        _controller = new UserTokenController(MockAuthService.Object, MockAccountsService.Object);
    }
    
    [Test]
    public async Task LogIn_WithValidCredentials_ReturnsOk()
    {
        // Mock existing user and auth token
        var account = Account.NewAccount("test_user", "test@email.com", "test_password");
        MockAccountsService.Setup(service => service.GetByUsernameAsync("test_user")).ReturnsAsync(account);
        MockAuthService.Setup(service => service.GenerateToken(account)).Returns("test_auth_token");
        
        // Call view method
        var timestamp = DateTime.Now;
        var result = await _controller.LogIn(new LogInRequestSchema() {Username = "test_user", Password = "test_password"});
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        
        // Validate response
        var resultContent = (result as OkObjectResult)?.Value as LogInResponseSchema;
        Assert.That(resultContent?.Token, Is.EqualTo("test_auth_token"));
        
        // Verify side effects
        Assert.That(account.LastLogin > timestamp);
    }
    
    [Test]
    public async Task LogIn_WithUnknownUsername_ReturnsUnauthorized()
    {
        // Call view method
        var result = await _controller.LogIn(new LogInRequestSchema() {Username = "test_user", Password = "test_password"});
        
        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }
    
    [Test]
    public async Task LogIn_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Mock existing user
        var account = Account.NewAccount("test_user", "test@email.com", "test_password");
        MockAccountsService.Setup(service => service.GetByUsernameAsync("test_user")).ReturnsAsync(account);
        
        // Call view method
        var result = await _controller.LogIn(new LogInRequestSchema() {Username = "test_user", Password = "invalid_password"});
        
        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }
}