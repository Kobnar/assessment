using System.Security.Claims;
using Assessment.Controllers;
using Assessment.Models;
using Assessment.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Assessment.Tests;

[TestFixture]
public class UserAccountControllerTests : ControllerTestFixture
{
    private UserAccountController _controller;

    [SetUp]
    public new void SetUp()
    {
        _controller = new UserAccountController(MockAccountsService.Object, MockProfilesService.Object);
    }

    [Test]
    public async Task SignUp_WithValidCredentials_ReturnsCreated()
    {
        // Mock user creation
        const string expectedUserId = "675f8a86ac7d8961533eb250";
        MockAccountsService.Setup(
            service => service.CountSignUpConflicts(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(0);
        MockAccountsService.Setup(
            service => service.CreateAsync(It.IsAny<Account>()))
            .Callback<Account>(input => { input.Id = expectedUserId; });
        
        // Call view method
        var result = await _controller.SignUp(
            new SignUpRequestSchema() {Username = "test_user", Email = "test@email.com", Password = "test_password"});
        Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
        
        // Validate response
        var account = (result as CreatedAtActionResult)?.Value as Account;
        Assert.That(account?.Id, Is.EqualTo(expectedUserId));
    }

    [Test]
    public async Task SignUp_WithShortUsername_ReturnsBadRequest()
    {
        MockAccountsService.Setup(
            service => service.CountSignUpConflicts(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(0);
        
        // Call view method
        var result = await _controller.SignUp(
            new SignUpRequestSchema() {Username = "abc", Email = "test@email.com", Password = "test_password"});
        Assert.That(result, Is.InstanceOf<BadRequestResult>());
    }

    [Test]
    public async Task SignUp_WithShortPassword_ReturnsBadRequest()
    {
        MockAccountsService.Setup(
            service => service.CountSignUpConflicts(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(0);
        
        // Call view method
        var result = await _controller.SignUp(
            new SignUpRequestSchema() {Username = "test_user", Email = "test@email.com", Password = "abc"});
        Assert.That(result, Is.InstanceOf<BadRequestResult>());
    }

    [Test]
    public async Task SignUp_WithInvalidEmail_ReturnsBadRequest()
    {
        MockAccountsService.Setup(
            service => service.CountSignUpConflicts(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(0);
        
        // Call view method
        var result = await _controller.SignUp(
            new SignUpRequestSchema() {Username = "test_user", Email = "invalid_email", Password = "test_password"});
        Assert.That(result, Is.InstanceOf<BadRequestResult>());
    }

    [Test]
    public async Task SignUp_WithExistingUser_ReturnsConflict()
    {
        MockAccountsService.Setup(
            service => service.CountSignUpConflicts(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(1);
        
        // Call view method
        var result = await _controller.SignUp(
            new SignUpRequestSchema() {Username = "test_user", Email = "test@email.com", Password = "test_password"});
        Assert.That(result, Is.InstanceOf<ConflictResult>());
    }

    [Test]
    public async Task ModifyAccount_WithValidData_ReturnsOk()
    {
        Assert.Fail("TODO");
    }

    [Test]
    public async Task ModifyAccount_WithNewEmail_UpdatesProfile()
    {
        const string userId = "675f8a86ac7d8961533eb250";
        var account = Account.NewAccount("test_user", "test@email.com", "test_password", userId);
        var profile = Profile.NewProfile(
            "test@email.com",
            new Name() { First = "John", Last = "Dowe" },
            "1234567890",
            new Address() { Line1 = "line1", City = "city", Country = "country", State = "ST", PostalCode = "12345" },
            account.Id);
        
        // Configure mocked services
        MockAccountsService.Setup(
            service => service.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(account);
        MockProfilesService.Setup(
            service => service.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(profile);
        
        // Set the ID on the controller's user
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId)};
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Mock HttpContext and User
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _controller.ControllerContext.HttpContext = httpContext;

        // Call view method
        var result = await _controller.ModifyAccount(
            new UpdateUserRequestSchema() {Email = "modified@email.com"});
        Assert.That(result, Is.InstanceOf<ActionResult<Account>>());
        
        // Validate side effect on profile
        // TODO: Find out how to check this w/ mock
        Assert.That(profile.Email, Is.EqualTo("modified@email.com"));
    }
}