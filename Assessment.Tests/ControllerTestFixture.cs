using Assessment.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Assessment.Tests;

/// <summary>
/// Encapsulates the common tasks of mocking specific services.
/// </summary>
public class ControllerTestFixture
{
    protected Mock<IAuthService> MockAuthService;
    protected Mock<IAccountsService> MockAccountsService;
    protected Mock<IProfilesService> MockProfilesService;
    
    [SetUp]
    public void SetUp()
    {
        MockAuthService = new Mock<IAuthService>();
        MockAccountsService = new Mock<IAccountsService>();
        MockProfilesService = new Mock<IProfilesService>();
    }
}