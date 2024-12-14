using System.Security.Claims;
using Assessment.Forms;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Controllers;

[ApiController]
[Route("account/token")]
public class UserTokenController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly AccountsService _accountsService;

    public UserTokenController(AuthService authService, AccountsService accountsService)
    {
        _authService = authService;
        _accountsService = accountsService;
    }
    
    [HttpPost]
    public async Task<IActionResult> LogIn(LogInForm authData)
    {
        // Query the user based on provided username
        var user = await _accountsService.GetByUsernameAsync(authData.Username);
        
        // User must exist
        if (user is null)
            return Unauthorized();
        
        // Provided password must be correct
        if (!user.VerifyPassword(authData.Password))
            return Unauthorized();
        
        // Update login timestamp
        user.LastLogin = DateTime.Now;
        await _accountsService.UpdateAsync(user);
        
        // Generate and return a new JWT
        var token = _authService.GenerateToken(user);
        return Ok(new { token = token });
    }

    /// <summary>
    /// A hacky endpoint that turns any authenticated user into an admin. In a real application, I would create some
    /// kind of command line interface to create an admin user. This is just for demonstration.
    /// </summary>
    /// <returns></returns>
    [HttpPost("makeadmin")]
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