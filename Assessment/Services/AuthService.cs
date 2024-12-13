using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Assessment.Models;
using Assessment.Settings;

namespace Assessment.Services;

public class AuthService
{
    
    private readonly AuthSettings _authSettings;

    public AuthService(IOptions<AuthSettings> jwtSettings)
    {
        _authSettings = jwtSettings.Value;
    }

    public string GenerateToken(Account userAccount)
    {
        if (userAccount.Id is null)
            return string.Empty;
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userAccount.Id),
            new Claim("groups", String.Join(",", userAccount.Groups))
            // new Claim(ClaimTypes.Email, user.Email),
            // Add any additional claims here
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _authSettings.Issuer,
            _authSettings.Audience,
            claims,
            expires: DateTime.Now.AddMinutes(_authSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.SecretKey));
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = _authSettings.Issuer,
                ValidAudience = _authSettings.Audience,
                IssuerSigningKey = key
            }, out var validatedToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}