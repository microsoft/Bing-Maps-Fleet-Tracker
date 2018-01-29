using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Trackable.Web.Auth
{
    public static class ClaimsReader
    {
        public static string ReadEmail(ClaimsPrincipal user)
        {
            var email = user.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;

            if (email == null)
            {
                email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            }

            return email;
        }

        public static string ReadName(ClaimsPrincipal user)
        {
            var name = user.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

            if (name == null)
            {
                name = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            }

            return name;
        }

        public static string ReadAudience(ClaimsPrincipal user)
        {
            var audience = user.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;

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
