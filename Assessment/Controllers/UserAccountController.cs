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

}