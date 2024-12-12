using Assessment.Forms;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Assessment.Controllers;

[ApiController]
[Route("/auth")]
public class AAuthenticationController : ControllerBase
{
    private readonly AAuthService _authService;
    private readonly AUsersService _usersService;

    public AAuthenticationController(AAuthService authService, AUsersService usersService)
    {
        _authService = authService;
        _usersService = usersService;
    }
    
    [HttpPost]
    public async Task<IActionResult> LogIn(AAuthForm authData)
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