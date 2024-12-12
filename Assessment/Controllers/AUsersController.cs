using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Assessment.Forms;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Controllers;

[Authorize]
[ApiController]
[Route("/users")]
public class AUsersController : ControllerBase
{
    private readonly AUsersService _usersService;

    public AUsersController(AUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp(ANewUserForm newUserData)
    {
        var newUser = AUser.NewUser(newUserData.Username, newUserData.Password);
        await _usersService.CreateAsync(newUser);

        return CreatedAtAction(nameof(GetAccountDetail), new { id = newUser.Id }, newUser);
    }

    [HttpGet("{userId:length(24)}")]
    public async Task<ActionResult<AUser>> GetAccountDetail(string userId)
    {
        // TODO: Create some kind of access policy
        var claimedId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (claimedId != userId)
            return Unauthorized();
        
        var user = await _usersService.GetByIdAsync(userId);
        if (user is null)
            return NotFound();
        return user;
    }
}