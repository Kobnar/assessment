using Microsoft.AspNetCore.Authorization;

namespace Assessment.Authentication;

public class ScopeRequirement : IAuthorizationRequirement
{
    public string RequiredScope { get; }

    public ScopeRequirement(string requiredScope)
    {
        RequiredScope = requiredScope;
    }
}