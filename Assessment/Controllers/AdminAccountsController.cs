using System.Security.Claims;
using Assessment.Forms;
using Assessment.Models;
using Assessment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Controllers;

/// <summary>
/// A controller for administrators to manage (mostly view) user accounts. Provides methods to query accounts,
/// review specific details about an account, and delete an account.
/// </summary>
[Authorize(Policy = "AdminScopePolicy")]
[ApiController]
[Route("admin/accounts")]
public class AdminAccountsController : ControllerBase
{
    private readonly IAccountsService _accountsService;

    public AdminAccountsController(IAccountsService accountsService)
    {
        _accountsService = accountsService;
    }

    [HttpGet]
    public async Task<ActionResult<QueryResult<Account>>> GetManyAccounts([FromQuery] QueryAccountsForm queryAccountsForm)
    {
        QueryResult<Account> accounts = await _accountsService.GetManyAsync(
            queryAccountsForm.Username,
            queryAccountsForm.Email,
            queryAccountsForm.CreatedAfter,
            queryAccountsForm.CreatedBefore,
            queryAccountsForm.Limit,
            queryAccountsForm.Skip
            );

        return accounts;
    }

    [HttpGet("{userId:length(24)}")]
    public async Task<ActionResult<Account>> GetAccountDetail(string userId)
    {
        var account = await _accountsService.GetByIdAsync(userId);
        if (account is null)
            return NotFound();
        
        return account;
    }

    [HttpDelete("{userId:length(24)}")]
    public async Task<IActionResult> DeleteAccount(string userId)
    {
        await _accountsService.DeleteAsync(userId);

        return NoContent();
    }
}