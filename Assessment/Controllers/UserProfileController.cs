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
[Route("profile")]
public class UserProfileController : ControllerBase
{
    private readonly ProfilesService _profilesService;

    public UserProfileController(AccountsService accountsService, ProfilesService profilesService)
    {
        _profilesService = profilesService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile(CreateProfileForm newProfileData)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        string? userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (userId is null || userEmail is null)
            return NotFound();

        Profile? existingProfile = await _profilesService.GetByIdAsync(userId);
        if (existingProfile is not null)
            return Conflict();
        
        var newProfile = Profile.NewProfile(
            userEmail, newProfileData.Name, newProfileData.PhoneNumber, newProfileData.Address, userId);
        await _profilesService.CreateAsync(newProfile);

        return CreatedAtAction(nameof(ViewProfileDetail), new { userId = newProfile.Id }, newProfile);
    }

    [HttpGet]
    public async Task<ActionResult<Profile>> ViewProfileDetail()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return NotFound();
        
        var profile = await _profilesService.GetByIdAsync(userId);
        if (profile is null)
            return NotFound();
        
        return profile;
    }

    [HttpPatch]
    public async Task<ActionResult<Profile>> ModifyProfileDetail(UpdateProfileForm updatedProfileData)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return NotFound();
        
        var profile = await _profilesService.GetByIdAsync(userId);
        if (profile is null)
            return NotFound();
        
        if (updatedProfileData.Name is not null)
            profile.Name = updatedProfileData.Name;
        
        if (updatedProfileData.PhoneNumber is not null)
            profile.PhoneNumber = updatedProfileData.PhoneNumber;
        
        if (updatedProfileData.Address is not null)
            profile.Address = updatedProfileData.Address;
        
        await _profilesService.UpdateAsync(profile);
        profile = await _profilesService.GetByIdAsync(userId);
        
        // TODO: Handle the case or return meaningful error
        if (profile is null)
            return Conflict();

        return profile;
    }
}