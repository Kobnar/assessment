using System.Security.Claims;
using Assessment.Forms;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Controllers;

[Authorize]
[ApiController]
[Route("admin/profiles")]
public class AdminProfilesController : ControllerBase
{
    private readonly ProfilesService _profilesService;

    public AdminProfilesController(ProfilesService profilesService)
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