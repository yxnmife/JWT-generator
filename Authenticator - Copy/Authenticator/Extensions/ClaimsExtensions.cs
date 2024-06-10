using System.Security.Claims;

namespace Authenticator.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetID(this ClaimsPrincipal user)
        {
            return user.Claims.SingleOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;
        }
    }
}

