// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
using Trackable.Web.Dtos;

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

        /// <summary>
        /// Redirects to login page
        /// </summary>
        /// <param name="redirectUri">Uri to be redirected to after signin is complete</param>
        /// <returns>Redirect Response</returns>
        [AllowAnonymous]
        [HttpGet("login")]
        public async Task Login(string redirectUri = "")
        {
            await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = $"/api/users/logincallback?redirectUri={redirectUri}" });
        }

        /// <summary>
        /// Call back invoked after successful sign in
        /// </summary>
        /// <param name="redirectUri">Uri to be redirected to after sign in processing is complete</param>
        /// <returns>Redirect Response</returns>
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

        /// <summary>
        /// Redirect to logout page
        /// </summary>
        /// <param name="redirectUri">Uri to redirect to after signout is complete</param>
        /// <returns>Redirect Response</returns>
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

        /// <summary>
        /// End the current session
        /// </summary>
        /// <param name="redirectUri">Uri to redirect to after session is closed</param>
        /// <returns>Redirect Response</returns>
        [HttpGet("endsession")]
        [Authorize(UserRoles.Blocked)]
        public async Task<IActionResult> EndSession(string redirectUri = "")
        {
            // If AAD sends a single sign-out message to the app, end the user's session,
            // but don't redirect to AAD for sign out.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect(redirectUri);
        }

        /// <summary>
        /// Query users
        /// </summary>
        /// <returns>List of users</returns>
        [HttpGet]
        [Authorize(UserRoles.Administrator)]
        public async Task<IEnumerable<UserDto>> Get()
        {
            var results = await this.userService.ListAsync();

            return this.dtoMapper.Map<IEnumerable<UserDto>>(results);
        }

        /// <summary>
        /// Get the current signed-in User
        /// </summary>
        /// <returns>The current user</returns>
        [HttpGet("me")]
        [Authorize(UserRoles.Blocked)]
        public async Task<UserDto> GetMe()
        {
            var result = await this.userService.GetUserByEmailAsync(ClaimsReader.ReadEmail(this.User));

            return this.dtoMapper.Map<UserDto>(result);
        }

        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>The user</returns>
        [HttpGet("{userId}")]
        [Authorize(UserRoles.Administrator)]
        public async Task<UserDto> GetUser(string userId)
        {
            var result = await this.userService.GetAsync(userId);

            return this.dtoMapper.Map<UserDto>(result);
        }

        /// <summary>
        /// Get or Create the PAT token of the current signed in user
        /// </summary>
        /// <param name="regenerateToken">True if existing token is to be revoked and a new one generated, false returns existing token</param>
        /// <returns>PAT token</returns>
        [HttpPost("me/token")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<JsonResult> GetToken(bool regenerateToken = false)
        {
            var user = await this.userService.GetUserByEmailAsync(ClaimsReader.ReadEmail(this.User));

            var token = await this.tokenService.GetLongLivedUserToken(user, regenerateToken);

            return Json(token);
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="user">The user details</param>
        /// <returns>The created user</returns>
        [HttpPost]
        [Authorize(UserRoles.Administrator)]
        public async Task<UserDto> Post([FromBody]UserDto user)
        {
            var model = this.dtoMapper.Map<User>(user);

            var result = await this.userService.AddAsync(model);

            return this.dtoMapper.Map<UserDto>(result);
        }

        /// <summary>
        /// Update existing user
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="userJson">The user details</param>
        /// <returns>The updated user</returns>
        [HttpPut("{userId}")]
        [Authorize(UserRoles.Administrator)]
        [ProducesResponseType(typeof(UserDto), 200)]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody]UserDto userJson)
        {
            var user = await this.userService.GetAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            if (user.Role.Name == UserRoles.Owner)
            {
                return BadRequest();
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

        /// <summary>
        /// Delete user and revoke user PATs
        /// </summary>
        /// <param name="userId">The user Id</param>
        /// <returns>Ok response</returns>
        [HttpDelete("{userId}")]
        [Authorize(UserRoles.Administrator)]
        public async Task<IActionResult> DeleteUser(string userId)
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

        /// <summary>
        /// Page used if error occured during authentication process
        /// </summary>
        /// <returns></returns>
        [HttpGet("accessdenied")]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return Forbid();
        }
    }
}