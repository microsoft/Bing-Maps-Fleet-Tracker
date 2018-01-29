using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Trackable.Models;
using Trackable.Repositories;

namespace Trackable.Services
{
    class JwtTokenService : CrudServiceBase<Guid, JwtToken, ITokenRepository>, ITokenService
    {
        private readonly SigningCredentials securityCredentials;

        public JwtTokenService(
            SigningCredentials securityCredentials,
            ITokenRepository tokenRepository)
            :base(tokenRepository)
        {
            this.securityCredentials = securityCredentials;
        }

        public async Task<string> GetLongLivedUserToken(User user, bool regenerateToken)
        {
            // Get stored token if available
            if (!regenerateToken)
            {
                var jwtTokens = await this.repository.GetActiveByUserAsync(user);

                if (jwtTokens.Any())
                {
                    return this.TokenFromClaims(jwtTokens.First().Claims);
                }
            }

            var jwtToken = new JwtToken { UserId = user.Id, IsActive = true };

            var claims = new Claim[] {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jwtToken.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Iss, JwtAuthConstants.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud, JwtAuthConstants.UserAudience),
                new Claim(JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddYears(10)).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}")
            };

            jwtToken.Claims = claims;
            
            // Store generated token and disable old tokens
            await this.repository.DisableTokensByUserAsync(user);
            await this.repository.AddAsync(jwtToken);

            return this.TokenFromClaims(jwtToken.Claims);
        }

        public async Task<string> GetLongLivedDeviceToken(TrackingDevice device, bool regenerateToken)
        {
            // Get stored token if available
            if (!regenerateToken)
            {
                var jwtTokens = await this.repository.GetActiveByDeviceAsync(device);

                if (jwtTokens.Any())
                {
                    return this.TokenFromClaims(jwtTokens.First().Claims);
                }
            }

            var jwtToken = new JwtToken { TrackingDeviceId = device.Id, IsActive = true };

            var claims = new Claim[] {
                new Claim(ClaimTypes.Name, device.Name),
                new Claim(JwtRegisteredClaimNames.Sub, device.Id),
                new Claim(JwtRegisteredClaimNames.Jti, jwtToken.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, JwtAuthConstants.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud, JwtAuthConstants.DeviceAudience),
                new Claim(JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddYears(10)).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}")
            };

            jwtToken.Claims = claims;

            await this.repository.DisableTokensByDeviceAsync(device);
            await this.repository.AddAsync(jwtToken);

            return this.TokenFromClaims(jwtToken.Claims);
        }

        public string GetShortLivedDeviceRegistrationToken()
        {
            var claims = new Claim[] {
                new Claim(ClaimTypes.Name, "Anonymous"),
                new Claim(JwtRegisteredClaimNames.Iss, JwtAuthConstants.Issuer),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Aud, JwtAuthConstants.RegistrationAudience),
                new Claim(JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddMinutes(15)).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}")
            };

            return this.TokenFromClaims(claims);
        }

        public async Task DisableDeviceTokens(TrackingDevice device)
        {
            await this.repository.DisableTokensByDeviceAsync(device);
        }

        public async Task DisableUserTokens(User user)
        {
            await this.repository.DisableTokensByUserAsync(user);
        }

        private string TokenFromClaims(IEnumerable<Claim> claims)
        {
            return new JwtSecurityTokenHandler()
                        .WriteToken(new JwtSecurityToken(new JwtHeader(this.securityCredentials), new JwtPayload(claims)));
        }
    }
}
