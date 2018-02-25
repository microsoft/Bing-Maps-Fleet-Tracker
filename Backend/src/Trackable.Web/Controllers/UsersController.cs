using AutoMapper;
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
using Trackable.Web.DTOs;

namespace Trackable.Web.Controllers
{
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ITokenService tokenService;
        private readonly IMapper dtoMapper;

        public UsersController(
            IUserService userService,
            ILoggerFactory loggerFactory,
            ITokenService tokenService,
            IMapper dtoMapper)
            : base(loggerFactory)
        {
            this.userService = userService;
            this.tokenService = tokenService;
            this.dtoMapper = dtoMapper;
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
        public async Task<IEnumerable<UserDto>> Get()
        {
            var results = await this.userService.ListAsync();

            return this.dtoMapper.Map<IEnumerable<UserDto>>(results);
        }

        [HttpGet("me")]
        [Authorize(UserRoles.Blocked)]
        public async Task<UserDto> GetMe()
        {
            var result = await this.userService.GetUserByEmailAsync(ClaimsReader.ReadEmail(this.User));

            return this.dtoMapper.Map<UserDto>(result);
        }

        [HttpGet("{userId}")]
        [Authorize(UserRoles.Administrator)]
        public async Task<UserDto> GetUser(Guid userId)
        {
            var result = await this.userService.GetAsync(userId);

            return this.dtoMapper.Map<UserDto>(result);
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
        public async Task<UserDto> Post([FromBody]UserDto user)
        {
            var model = this.dtoMapper.Map<User>(user);

            var result = await this.userService.AddAsync(model);

            return this.dtoMapper.Map<UserDto>(result);
        }

        [HttpPut("{userId}")]
        [Authorize(UserRoles.Administrator)]
        public async Task<IActionResult> SetRole(Guid userId, [FromBody]UserDto userJson)
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

            var result = await this.userService.UpdateAsync(userId, user);

            return Json(this.dtoMapper.Map<UserDto>(result));
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