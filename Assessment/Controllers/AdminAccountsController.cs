using System.Security.Claims;
using Assessment.Forms;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Controllers;

[Authorize]
[ApiController]
[Route("admin/accounts")]
public class AdminAccountsController : ControllerBase
{
    private readonly AccountsService _accountsService;

    public AdminAccountsController(AccountsService accountsService)
    {
        _accountsService = accountsService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp(SignUpForm newAccountData)
    {
        // TODO: Validate zero-length usernames
        if (newAccountData.Username.Length == 0)
            return BadRequest("Username is required");
        
        // TODO: Make this more efficient (we don't need the entire user doc if it exists)
        var existingUser = await _accountsService.GetByUsernameAsync(newAccountData.Username);
        if (existingUser is not null)
            return Conflict();
        
        var newAccount = Account.NewAccount(newAccountData.Username, newAccountData.Email, newAccountData.Password);
        await _accountsService.CreateAsync(newAccount);

        return CreatedAtAction(nameof(GetAccountDetail), new { userId = newAccount.Id }, newAccount);
    }

    [HttpGet("{userId:length(24)}")]
    public async Task<ActionResult<Account>> GetAccountDetail(string userId)
    {
        // TODO: Create some kind of access policy
        var claimedId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (claimedId != userId)
            return Unauthorized();
        
        var account = await _accountsService.GetByIdAsync(userId);
        if (account is null)
            return NotFound();
        
        return account;
    }

    [HttpPut("{userId:length(24)}")]
    public async Task<ActionResult<Account>> UpdateAccountDetail(string userId, UpdateUserForm updateAccountData)
    {
        // TODO: Create some kind of access policy
        var claimedId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (claimedId != userId)
            return Unauthorized();
        
        var account = await _accountsService.GetByIdAsync(userId);
        if (account is null)
            return NotFound();

        if (updateAccountData.Username is not null)
        {
            if (updateAccountData.Username.Length == 0)
                return BadRequest("Username cannot be empty");
            account.Username = updateAccountData.Username;
        }
        
        await _accountsService.UpdateAsync(account);
        
        return account;
    }

    [HttpPost("{userId:length(24)}/password")]
    public async Task<IActionResult> SetPassword(string userId, SetPasswordForm newPasswordData)
    {
        // TODO: Create some kind of access policy
        var claimedId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (claimedId != userId)
            return Unauthorized();
        
        var account = await _accountsService.GetByIdAsync(userId);
        if (account is null)
            return NotFound();
        
        if (!account.VerifyPassword(newPasswordData.OldPassword))
            return Unauthorized();
        
        account.SetPassword(newPasswordData.NewPassword);
        await _accountsService.UpdateAsync(account);
        
        return NoContent();
    }
}