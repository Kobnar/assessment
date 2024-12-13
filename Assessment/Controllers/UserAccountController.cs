using System.Security.Claims;
using Assessment.Forms;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Controllers;

[Authorize]
[ApiController]
[Route("account")]
public class UserAccountController : ControllerBase
{
    private readonly AccountsService _accountsService;

    public UserAccountController(AccountsService accountsService)
    {
        _accountsService = accountsService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp(SignUpForm newAccountData)
    {
        // TODO: Make this more efficient (we don't need the entire user doc if it exists)
        Account? existingAccount = await _accountsService.GetByUsernameAsync(newAccountData.Username);
        if (existingAccount is not null)
            return Conflict();
        
        // TODO: Make this more efficient (we don't need the entire user doc if it exists)
        existingAccount = await _accountsService.GetByEmailAsync(newAccountData.Email);
        if (existingAccount is not null)
            return Conflict();
        
        var newAccount = Account.NewAccount(newAccountData.Username, newAccountData.Email, newAccountData.Password);
        await _accountsService.CreateAsync(newAccount);

        return CreatedAtAction(nameof(GetAccountDetail), new { userId = newAccount.Id }, newAccount);
    }

    [HttpGet]
    public async Task<ActionResult<Account>> GetAccountDetail()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return NotFound();
        
        var account = await _accountsService.GetByIdAsync(userId);
        if (account is null)
            return NotFound();
        
        return account;
    }
}