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

    [HttpGet]
    public async Task<ActionResult<List<Account>>> GetManyAccounts([FromQuery] QueryAccountsForm queryAccountsForm)
    {
        // TODO: Create some kind of access policy for this
        var jwtScope = User.FindFirstValue("scope");
        if (jwtScope != "admin")
            return Unauthorized();
        
        List<Account> accounts = await _accountsService.GetManyAsync(
            queryAccountsForm.Username,
            queryAccountsForm.Email,
            queryAccountsForm.CreatedAfter,
            queryAccountsForm.CreatedBefore
            );

        return accounts;
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
}