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
    public async Task<IActionResult> Post(ANewUserForm newUserData)
    {
        var newUser = AUser.NewUser(newUserData.Username, newUserData.Password);
        await _usersService.CreateAsync(newUser);

        return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<AUser>> Get(string id)
    {
        var user = await _usersService.GetAsync(id);
        if (user is null)
            return NotFound();
        return user;
    }
    
    [HttpPost("{id:length(24)}/verify")]
    public async Task<ActionResult<AUser>> VerifyPassword(string id, [FromBody] ANewUserForm newUserData)
    {
        var user = await _usersService.GetAsync(id);
        if (user is null)
            return NotFound();
        if (!user.VerifyPassword(newUserData.Password))
            return BadRequest();
        return user;
    }
}