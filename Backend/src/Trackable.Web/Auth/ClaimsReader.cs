using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Trackable.Web.Auth
{
    public static class ClaimsReader
    {
        public static string ReadEmail(ClaimsPrincipal user)
        {
            // AAD V1 Claim
            var email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Upn)?.Value;

            // AAD V2 Claim
            if (email == null)
            {
                email = user.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            }

            // JWT Claim
            if (email == null)
            {
                email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            }

            return email;
        }

        public static string ReadName(ClaimsPrincipal user)
        {
            // AAD V2 Claim
            var name = user.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

            // AAD V1 & JWT Claim
            if (name == null)
            {
                name = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            }

            return name;
        }

        public static string ReadAudience(ClaimsPrincipal user)
        {
            // AAD V2 Claim
            var audience = user.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;

            // AAD V1 & JWT Claim
            if (audience == null)
            {
                audience = user.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Aud)?.Value;
            }

            return audience;
        }

        public static string ReadSubject(ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public static string ReadTokenId(ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
        }
    }
}
