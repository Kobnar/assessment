using Assessment.Forms;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Assessment.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly AccountsService _accountsService;

    public AuthController(AuthService authService, AccountsService accountsService)
    {
        _authService = authService;
        _accountsService = accountsService;
    }
    
    [HttpPost]
    public async Task<IActionResult> LogIn(LogInForm authData)
    {
        var user = await _accountsService.GetByUsernameAsync(authData.Username);
        
        // User must exist
        if (user is null)
            return NotFound();
        
        // Provided password must be correct
        if (!user.VerifyPassword(authData.Password))
            return Unauthorized();
        
        // Update login timestamp
        user.LastLogin = DateTime.Now;
        await _accountsService.UpdateAsync(user);
        
        // Generate and return JWT
        var token = _authService.GenerateToken(user);
        return Ok(new { token = token });
    }
}