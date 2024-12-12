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
    private readonly AJwtService _jwtService;
    private readonly AUsersService _usersService;

    public AAuthenticationController(AJwtService jwtService, AUsersService usersService)
    {
        _jwtService = jwtService;
        _usersService = usersService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post(AAuthenticationForm authData)
    {
        var user = await _usersService.GetByUsernameAsync(authData.Username);
        
        if (user is null)
            return NotFound();
        
        else if (!user.VerifyPassword(authData.Password))
            return Unauthorized();
        
        var token = _jwtService.GenerateToken(user);
        return Ok(new { token = token });
    }
}