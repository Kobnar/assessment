using Microsoft.AspNetCore.Authorization;

namespace Assessment.Authentication;


public class ScopeRequirementHandler : AuthorizationHandler<ScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
    {
        var scopeClaim = context.User.FindFirst("scope")?.Value;

        if (!string.IsNullOrEmpty(scopeClaim) && scopeClaim.Split(' ').Contains(requirement.RequiredScope))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}