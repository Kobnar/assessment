using System.Security.Claims;
using Assessment.Forms;
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

    public UserAccountController(IAccountsService accountsService)
    {
        _accountsService = accountsService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp(SignUpForm newAccountData)
    {
        long nConflicts = await _accountsService.CountSignUpConflicts(newAccountData.Username, newAccountData.Email);
        if (0 < nConflicts)
            return Conflict();
        
        var newAccount = Account.NewAccount(newAccountData.Username, newAccountData.Email, newAccountData.Password);
        await _accountsService.CreateAsync(newAccount);

        return CreatedAtAction(nameof(ViewAccountDetail), new { userId = newAccount.Id }, newAccount);
    }

    [HttpGet]
    public async Task<ActionResult<Account>> ViewAccountDetail()
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
    public async Task<ActionResult<Account>> ModifyAccountDetail(UpdateUserForm updatedAccountData)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return NotFound();
        
        var account = await _accountsService.GetByIdAsync(userId);
        if (account is null)
            return NotFound();
        
        // Enforce uniqueness with username and email
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
            existingAccount = await _accountsService.GetByEmailAsync(updatedAccountData.Email);
            if (existingAccount is not null)
                return Conflict();
            
            account.Email = updatedAccountData.Email;
        }
        
        // Update and refresh record
        await _accountsService.UpdateAsync(account);
        account = await _accountsService.GetByIdAsync(userId);
        if (account is null)
            return Problem(); // Not sure what to do in this case

        return account;
    }

    [HttpPut("password")]
    public async Task<IActionResult> SetPassword(SetPasswordForm setPasswordData)
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