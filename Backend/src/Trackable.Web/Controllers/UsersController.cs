using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Models;
using Trackable.Services;
using Trackable.Web.Auth;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Trackable.Web.Controllers
{
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ITokenService tokenService;

        public UsersController(
            IUserService userService,
            ILoggerFactory loggerFactory,
            ITokenService tokenService)
            : base(loggerFactory)
        {
            this.userService = userService;
            this.tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpGet("login")]
        public async Task Login(string redirectUri = "")
        {
            await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = $"/api/users/logincallback?redirectUri={redirectUri}" });
        }

        [Authorize]
        [HttpGet("logincallback")]
        public async Task<IActionResult> LoginCallback(string redirectUri = "")
        {
            var email = ClaimsReader.ReadEmail(this.User);

            if (email == null)
            {
                throw new UnknownProviderException();
            }

            var user = await this.userService.GetOrCreateUserByEmailAsync(
                email,
                ClaimsReader.ReadName(this.User),
                ClaimsReader.ReadSubject(this.User));

            if (string.IsNullOrWhiteSpace(redirectUri))
            {
                return Ok();
            }
            else
            {
                return Redirect(redirectUri);
            }
        }

        [HttpGet("logout")]
        [Authorize(UserRoles.Blocked)]
        public async Task LogOut(string redirectUri = "")
        {
            if (this.User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = redirectUri });

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }

        [HttpGet("endsession")]
        [Authorize(UserRoles.Blocked)]
        public async Task<IActionResult> EndSession(string redirectUri = "")
        {
            // If AAD sends a single sign-out message to the app, end the user's session,
            // but don't redirect to AAD for sign out.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect(redirectUri);
        }

        [HttpGet]
        [Authorize(UserRoles.Administrator)]
        public async Task<IEnumerable<User>> Get()
        {
            return await this.userService.ListAsync();
        }

        [HttpGet("me")]
        [Authorize(UserRoles.Blocked)]
        public async Task<User> GetMe()
        {
            return await this.userService.GetUserByEmailAsync(ClaimsReader.ReadEmail(this.User));
        }

        [HttpGet("{userId}")]
        [Authorize(UserRoles.Administrator)]
        public async Task<User> GetUser(Guid userId)
        {
            return await this.userService.GetAsync(userId);
        }

        [HttpPost("me/token")]
        public async Task<JsonResult> GetToken(bool regenerateToken = false)
        {
            var user = await this.userService.GetUserByEmailAsync(ClaimsReader.ReadEmail(this.User));

            var token = await this.tokenService.GetLongLivedUserToken(user, regenerateToken);

            return Json(token);
        }

        [HttpPost]
        [Authorize(UserRoles.Administrator)]
        public async Task<User> Post([FromBody]User user)
        {
            return await this.userService.AddAsync(user);
        }

        [HttpPut("{userId}")]
        [Authorize(UserRoles.Administrator)]
        public async Task<IActionResult> SetRole(Guid userId, [FromBody]User userJson)
        {
            var user = await this.userService.GetAsync(userId);

            if (user == null)
            {
                return null;
            }

            if (user.Role.Name == UserRoles.Owner)
            {
                return Json(user);
            }

            if (UserRoles.Blocked.Equals(userJson.Role.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                user.Role = await this.userService.GetRoleAsync(UserRoles.Blocked);
            }
            else if (UserRoles.Viewer.Equals(userJson.Role.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                user.Role = await this.userService.GetRoleAsync(UserRoles.Viewer);
            }
            else if (UserRoles.Administrator.Equals(userJson.Role.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                user.Role = await this.userService.GetRoleAsync(UserRoles.Administrator);
            }
            else
            {
                return BadRequest();
            }

            return Json(await this.userService.UpdateAsync(userId, user));
        }

        [HttpDelete("{userId}")]
        [Authorize(UserRoles.Administrator)]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var user = await this.userService.GetAsync(userId);

            if (user == null || user.Role.Name == UserRoles.Owner)
            {
                return BadRequest();
            }

            await this.tokenService.DisableUserTokens(user);
            await this.userService.DeleteAsync(user.Id);

            return Ok();
        }

        [HttpGet("accessdenied")]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return Forbid();
        }
    }
}