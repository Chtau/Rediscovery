using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

public static class ClaimsExtensions
{
    public static string GetSid(this IEnumerable<Claim> claims)
    {
        return claims.Where(c => c.Type == ClaimTypes.Sid)
               .Select(c => c.Value).FirstOrDefault();
    }
}