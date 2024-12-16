using System.Security.Claims;
using Assessment.Schema;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Controllers;

/// <summary>
/// Provides a basic login view which issues a valid JWT.
/// </summary>
[ApiController]
[Route("account/token")]
public class UserTokenController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAccountsService _accountsService;

    public UserTokenController(IAuthService authService, IAccountsService accountsService)
    {
        _authService = authService;
        _accountsService = accountsService;
    }
    
    [HttpPost]
    public async Task<IActionResult> LogIn(LogInRequestSchema authData)
    {
        // Query the user based on provided username
        var account = await _accountsService.GetByUsernameAsync(authData.Username);
        
        // User must exist
        if (account is null)
            return Unauthorized();
        
        // Provided password must be correct
        if (!account.VerifyPassword(authData.Password))
            return Unauthorized();
        
        // Update login timestamp
        account.LastLogin = DateTime.Now;
        await _accountsService.UpdateAsync(account);
        
        // Generate and return a new JWT
        var token = _authService.GenerateToken(account);
        return Ok(new LogInResponseSchema() {Token = token});
    }

    /// <summary>
    /// A hacky endpoint that turns any authenticated user into an admin. In a real application, I would create some
    /// kind of command line interface to create an admin user. This is just for demonstration.
    /// </summary>
    /// <returns></returns>
    [HttpPost("make-admin")]
    public async Task<IActionResult> MakeAdmin()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return NotFound();
        
        Account? account = await _accountsService.GetByIdAsync(userId);
        if (account is null)
            return NotFound();
        
        account.IsAdmin = true;
        await _accountsService.UpdateAsync(account);
        
        return NoContent();
    }
}