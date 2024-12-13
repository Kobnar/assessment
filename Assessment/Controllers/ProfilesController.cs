using System.Security.Claims;
using Assessment.Forms;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ProfilesController : ControllerBase
{
    private readonly ProfilesService _profilesService;

    public ProfilesController(ProfilesService profilesService)
    {
        _profilesService = profilesService;
    }

    [HttpGet("{userId:length(24)}")]
    public async Task<ActionResult<Profile>> GetProfileDetail(string userId)
    {
        // TODO: Create some kind of access policy
        var claimedId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (claimedId != userId)
            return Unauthorized();

        var profile = await _profilesService.GetByIdAsync(userId);
        if (profile is null)
            return NotFound();

        return profile;
    }
}