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
    private readonly AccountsService _usersService;

    public AuthController(AuthService authService, AccountsService usersService)
    {
        _authService = authService;
        _usersService = usersService;
    }
    
    [HttpPost]
    public async Task<IActionResult> LogIn(LogInForm authData)
    {
        var user = await _usersService.GetByUsernameAsync(authData.Username);
        
        if (user is null)
            return NotFound();
        
        else if (!user.VerifyPassword(authData.Password))
            return Unauthorized();
        
        var token = _authService.GenerateToken(user);
        return Ok(new { token = token });
    }
}