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

    [HttpGet]
    public async Task<ActionResult<QueryResult<Profile>>> GetManyProfiles([FromQuery] QueryProfilesForm queryProfilesForm)
    {
        // TODO: Create some kind of access policy for this
        var jwtScope = User.FindFirstValue("scope");
        if (jwtScope != "admin")
            return Unauthorized();
        
        QueryResult<Profile> profiles = await _profilesService.GetManyAsync(
            queryProfilesForm.Name,
            queryProfilesForm.Email,
            queryProfilesForm.PhoneNumber,
            queryProfilesForm.CreatedAfter,
            queryProfilesForm.CreatedBefore,
            queryProfilesForm.Limit,
            queryProfilesForm.Skip
        );

        return profiles;
    }

    [HttpGet("{userId:length(24)}")]
    public async Task<ActionResult<Profile>> GetProfileDetail(string userId)
    {
        // TODO: Create some kind of access policy for this
        var jwtScope = User.FindFirstValue("scope");
        if (jwtScope != "admin")
            return Unauthorized();

        var profile = await _profilesService.GetByIdAsync(userId);
        if (profile is null)
            return NotFound();

        return profile;
    }
}