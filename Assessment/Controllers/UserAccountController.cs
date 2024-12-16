using System.Security.Claims;
using Assessment.Schema;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Controllers;

/// <summary>
/// A controller for a user to manage their own account. Provides methods to sign up, view their account, modify
/// details about their account (including change password), and delete their account.
/// </summary>
[Authorize]
[ApiController]
[Route("account")]
public class UserAccountController : ControllerBase
{
    private readonly IAccountsService _accountsService;
    private readonly IProfilesService _profilesService;

    public UserAccountController(IAccountsService accountsService, IProfilesService profilesService)
    {
        _accountsService = accountsService;
        _profilesService = profilesService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp(SignUpRequestSchema newAccountData)
    {
        long nConflicts = await _accountsService.CountSignUpConflicts(newAccountData.Username, newAccountData.Email);
        if (0 < nConflicts)
            return Conflict();
        
        var newAccount = Account.NewAccount(newAccountData.Username, newAccountData.Email, newAccountData.Password);
        await _accountsService.CreateAsync(newAccount);

        return CreatedAtAction(nameof(ViewAccount), new { userId = newAccount.Id }, newAccount);
    }

    [HttpGet]
    public async Task<ActionResult<Account>> ViewAccount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return NotFound();
        
        var account = await _accountsService.GetByIdAsync(userId);
        if (account is null)
            return NotFound();
        
        return account;
    }

    [HttpPatch]
    public async Task<ActionResult<Account>> ModifyAccount(UpdateUserRequestSchema updatedAccountData)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return NotFound();
        
        var account = await _accountsService.GetByIdAsync(userId);
        if (account is null)
            return NotFound();
        
        // Enforce uniqueness with username and email
        bool doProfileSync = false;
        Account? existingAccount;
        if (updatedAccountData.Username is not null)
        {
            existingAccount = await _accountsService.GetByUsernameAsync(updatedAccountData.Username);
            if (existingAccount is not null)
                return Conflict();
            
            account.Username = updatedAccountData.Username;
        }
        if (updatedAccountData.Email is not null)
        {
            doProfileSync = true;
            existingAccount = await _accountsService.GetByEmailAsync(updatedAccountData.Email);
            if (existingAccount is not null)
                return Conflict();
            
            account.Email = updatedAccountData.Email;
        }
        
        // Update account and profile
        await _accountsService.UpdateAsync(account);
        if (doProfileSync)
            await _profilesService.SyncWithAccount(account);

        return account;
    }

    [HttpPut("password")]
    public async Task<IActionResult> SetPassword(SetPasswordRequestSchema setPasswordData)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return NotFound();

        var account = await _accountsService.GetByIdAsync(userId);
        if (account is null)
            return NotFound();

        if (!account.VerifyPassword(setPasswordData.OldPassword))
            return Unauthorized();

        account.SetPassword(setPasswordData.NewPassword);
        await _accountsService.UpdateAsync(account);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAccount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return NotFound();
        
        await _accountsService.DeleteAsync(userId);

        return NoContent();
    }
}