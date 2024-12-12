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
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly AccountsService _accountsService;

    public AccountsController(AccountsService accountsService)
    {
        _accountsService = accountsService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp(SignUpForm newAccountData)
    {
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
}