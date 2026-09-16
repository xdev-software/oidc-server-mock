using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Duende.IdentityModel;
using Duende.IdentityServer.Test;

namespace OpenIdConnectServerMock.Test;

// Extends base and exposes a getter for the underlying list
public class MutableTestUserStore : TestUserStore
{
    // Can't use a dictionary/HashMap here because people like to hardcode everything and make it non-overrideable...
    protected List<TestUser> Users { get; }

    public MutableTestUserStore(List<TestUser> users) : base(users)
    {
        Users = users;
    }

    protected int FindIndexForSubjectId(string subjectId)
    {
        return Users.FindIndex(u => u.SubjectId == subjectId);
    }

    // Derived from AutoProvisionUser (super)
    public List<Claim> DetermineClaims(ICollection<Claim> claims)
    {
        // create a list of claims that we want to transfer into our store
        var filtered = new List<Claim>();

        foreach (var claim in claims)
        {
            // if the external system sends a display name - translate that to the standard OIDC name claim
            if (claim.Type == ClaimTypes.Name)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, claim.Value));
            }
            // if the JWT handler has an outbound mapping to an OIDC claim use that
            else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.TryGetValue(claim.Type, out var value))
            {
                filtered.Add(new Claim(value, claim.Value));
            }
            // copy the claim as-is
            else
            {
                filtered.Add(claim);
            }
        }

        // if no display name was provided, try to construct by first and/or last name
        if (!filtered.Any(x => x.Type == JwtClaimTypes.Name))
        {
            var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
            var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
            if (first != null && last != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, first + ' ' + last));
            }
            else if (first != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, first));
            }
            else if (last != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, last));
            }
        }

        return filtered;
    }

    public bool ReplaceUser(string subjectId, TestUser testUser)
    {
        int index = FindIndexForSubjectId(subjectId);
        if (index == -1)
            return false;

        Users[index] = testUser;
        return true;
    }

    public bool RemoveUser(string subjectId)
    {
        int index = FindIndexForSubjectId(subjectId);
        if (index == -1)
            return false;

        Users.RemoveAt(index);
        return true;
    }
}
