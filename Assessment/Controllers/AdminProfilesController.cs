using Assessment.Schema;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Controllers;

/// <summary>
/// A controller for administrators to manage user profiles. Provides methods to query profiles,
/// review specific details about a profile, modify a profile, and delete a profile.
/// </summary>
[Authorize(Policy = "AdminScopePolicy")]
[ApiController]
[Route("admin/profiles")]
public class AdminProfilesController : ControllerBase
{
    private readonly IProfilesService _profilesService;

    public AdminProfilesController(IProfilesService profilesService)
    {
        _profilesService = profilesService;
    }

    [HttpGet]
    public async Task<ActionResult<QueryResult<Profile>>> GetManyProfiles([FromQuery] QueryProfilesRequestSchema queryProfilesForm)
    {
        QueryResult<Profile> profiles = await _profilesService.GetManyAsync(
            queryProfilesForm.Name,
            queryProfilesForm.Email,
            queryProfilesForm.PhoneNumber,
            queryProfilesForm.City,
            queryProfilesForm.State,
            queryProfilesForm.PostalCode,
            queryProfilesForm.CreatedAfter,
            queryProfilesForm.CreatedBefore,
            queryProfilesForm.Limit,
            queryProfilesForm.Skip
        );

        return profiles;
    }

    [HttpGet("{userId:length(24)}")]
    public async Task<ActionResult<Profile>> GetProfile(string userId)
    {
        var profile = await _profilesService.GetByIdAsync(userId);
        if (profile is null)
            return NotFound();

        return profile;
    }

    [HttpPatch("{userId:length(24)}")]
    public async Task<ActionResult<Profile>> UpdateProfile(string userId, UpdateProfileRequestSchema updatedProfileData)
    {
        var profile = await _profilesService.GetByIdAsync(userId);
        if (profile is null)
            return NotFound();

        // There's probably a better way to injest sparse fields from the client and set only those fields on the
        // document, but this works and I can refine that kind of implementation detail with more time.
        
        if (updatedProfileData.Name is not null)
        {
            UpdateNameRequestSchema nameData = updatedProfileData.Name;
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
            UpdateAddressRequestSchema addressData = updatedProfileData.Address;
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

    [HttpDelete("{userId:length(24)}")]
    public async Task<IActionResult> DeleteProfile(string userId)
    {
        await _profilesService.DeleteAsync(userId);

        return NoContent();
    }
}