using System.Security.Claims;
using Assessment.Forms;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Controllers;

/// <summary>
/// A controller for a user to manage their own profile. Provides methods to create a profile, view and modify
/// details about their profile, and delete their profile.
/// </summary>
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

        long nConflicts = await _profilesService.CountByIdAsync(userId);
        if (0 < nConflicts)
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

        // There's probably a better way to injest sparse fields from the client and set only those fields on the
        // document, but this works and I can refine that kind of implementation detail with more time.
        
        if (updatedProfileData.Name is not null)
        {
            UpdateNameForm nameData = updatedProfileData.Name;
            if (nameData.First is not null)
                profile.Name.First = nameData.First;
            if (nameData.Middle is not null)
                profile.Name.Middle = nameData.Middle;
            if (nameData.Last is not null)
                profile.Name.Last = nameData.Last;
        }
        
        if (updatedProfileData.PhoneNumber is not null)
            profile.PhoneNumber = updatedProfileData.PhoneNumber;

        if (updatedProfileData.Address is not null)
        {
            UpdateAddressForm addressData = updatedProfileData.Address;
            if (addressData.Line1 is not null)
                profile.Address.Line1 = addressData.Line1;
            if (addressData.Line2 is not null)
                profile.Address.Line2 = addressData.Line2;
            if (addressData.City is not null)
                profile.Address.City = addressData.City;
            if (addressData.State is not null)
                profile.Address.State = addressData.State;
            if (addressData.Country is not null)
                profile.Address.Country = addressData.Country;
            if (addressData.PostalCode is not null)
                profile.Address.PostalCode = addressData.PostalCode;
        }
        
        await _profilesService.UpdateAsync(profile);
        profile = await _profilesService.GetByIdAsync(userId);
        if (profile is null)
            return Problem(); // Not sure what to do in this case

        return profile;
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return NotFound();
        
        await _profilesService.DeleteAsync(userId);

        return NoContent();
    }
}