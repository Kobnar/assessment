using System.Security.Claims;
using Assessment.Models;

namespace Assessment.Services;

public interface IAuthService
{
    string GenerateToken(Account account);

    ClaimsPrincipal? ValidateToken(string token);
}