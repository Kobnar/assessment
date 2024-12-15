using Assessment.Controllers;
using Assessment.Forms;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Assessment.Tests;

[TestFixture]
public class UserTokenControllerTests
{
    Mock<IAuthService> _mockAuthService;
    Mock<IAccountsService> _mockAccountsService;
    UserTokenController _controller;

    [SetUp]
    public void Setup()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockAccountsService = new Mock<IAccountsService>();
        _controller = new UserTokenController(_mockAuthService.Object, _mockAccountsService.Object);
    }
    
    [Test]
    public async Task LogIn_WithUnknownUser_ReturnsUnauthorized()
    {
        // Mock IAccountsService will return null for GetByUsernameAsync
        var result = await _controller.LogIn(new LogInForm() {Username = "test_user", Password = "test_password"});
        
        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }
    
    [Test]
    public async Task LogIn_WithInvalidPassword_ReturnsUnauthorized()
    {
        string username = "test_user";
        string email = "test@email.com";
        string password = "test_password";
        Account account = Account.NewAccount(username, email, password);
        
        _mockAccountsService.Setup(service => service.GetByUsernameAsync(username)).ReturnsAsync(account);
        
        // Mock IAccountsService will return null for GetByUsernameAsync
        var result = await _controller.LogIn(new LogInForm() {Username = username, Password = "invalid_password"});
        
        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }
    
    [Test]
    public async Task LogIn_WithValidCredentials_ReturnsOk()
    {
        string username = "test_user";
        string email = "test@email.com";
        string password = "test_password";
        string authToken = "test_auth_token";
        Account account = Account.NewAccount(username, email, password);
        
        // Set up mocked side effects
        _mockAccountsService.Setup(service => service.GetByUsernameAsync(username)).ReturnsAsync(account);
        _mockAuthService.Setup(service => service.GenerateToken(account)).Returns(authToken);
        
        DateTime timestamp = DateTime.Now;
        var result = await _controller.LogIn(new LogInForm() {Username = username, Password = password});
        
        // Check response
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        // TODO: Assert valid token in response
        
        // Check side effects
        Assert.That(account.LastLogin > timestamp);
    }
}