// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using Trackable.Common;
using Trackable.Services;

namespace Trackable.Web.Auth
{
    public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
    {
        private readonly ITokenService tokenService;
        private readonly IUserService userService;

        public RoleRequirementHandler(IUserService userService, ITokenService tokenService)
        {
            this.tokenService = tokenService;
            this.userService = userService;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            // Allow filters to override each other by taking only the last authorization filter
            var resoureContext = (AuthorizationFilterContext)context.Resource;
            var lastFilter = (AuthorizeFilter)resoureContext.Filters.Last(f => f is AuthorizeFilter);
            var filterRequirement = lastFilter.Policy.Requirements.FirstOrDefault() as RoleRequirement;
            if (filterRequirement == null || filterRequirement != requirement)
            {
                context.Succeed(requirement);
                return;
            }

            // Find the user Role
            string userRole = UserRoles.Blocked;

            var audience = ClaimsReader.ReadAudience(context.User);
            if (audience == JwtAuthConstants.RegistrationAudience)
            {
                userRole = UserRoles.DeviceRegistration;
            }
            else if (audience == JwtAuthConstants.DeviceAudience)
            {
                userRole = UserRoles.TrackingDevice;

                // Validate jwtToken is not deactivated
                var jwtToken = await this.tokenService.GetAsync(Guid.Parse(ClaimsReader.ReadTokenId(context.User)));
                if (!jwtToken.IsActive)
                {
                    context.Fail();
                    return;
                }
            }
            else if (audience == JwtAuthConstants.UserAudience)
            {
                var id = ClaimsReader.ReadSubject(context.User);
                var user = await userService.GetAsync(Guid.Parse(id));
                userRole = user.Role.Name;

                // Validate jwtToken is not deactivated
                var jwtToken = await this.tokenService.GetAsync(Guid.Parse(ClaimsReader.ReadTokenId(context.User)));
                if (!jwtToken.IsActive)
                {
                    context.Fail();
                    return;
                }
            }
            else
            {
                var email = ClaimsReader.ReadEmail(context.User);
                if (email == null)
                {
                    context.Fail();
                    return;
                }
                var user = await userService.GetOrCreateUserByEmailAsync(
                   email,
                   ClaimsReader.ReadName(context.User),
                   ClaimsReader.ReadSubject(context.User));

                userRole = user.Role.Name;
            }

            switch (requirement.Role)
            {
                case UserRoles.Blocked:
                    if (userRole.Equals(UserRoles.Blocked)
                        || userRole.Equals(UserRoles.Pending)
                        || userRole.Equals(UserRoles.DeviceRegistration)
                        || userRole.Equals(UserRoles.TrackingDevice)
                        || userRole.Equals(UserRoles.Viewer)
                        || userRole.Equals(UserRoles.Administrator)
                        || userRole.Equals(UserRoles.Owner))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                    break;

                case UserRoles.Pending:
                    if (userRole.Equals(UserRoles.Pending)
                        || userRole.Equals(UserRoles.DeviceRegistration)
                        || userRole.Equals(UserRoles.TrackingDevice)
                        || userRole.Equals(UserRoles.Viewer)
                        || userRole.Equals(UserRoles.Administrator)
                        || userRole.Equals(UserRoles.Owner))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                    break;

                case UserRoles.DeviceRegistration:
                    if (userRole.Equals(UserRoles.DeviceRegistration)
                        || userRole.Equals(UserRoles.TrackingDevice)
                        || userRole.Equals(UserRoles.Viewer)
                        || userRole.Equals(UserRoles.Administrator)
                        || userRole.Equals(UserRoles.Owner))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                    break;

                case UserRoles.TrackingDevice:
                    if (userRole.Equals(UserRoles.TrackingDevice)
                        || userRole.Equals(UserRoles.Viewer)
                        || userRole.Equals(UserRoles.Administrator)
                        || userRole.Equals(UserRoles.Owner))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                    break;

                case UserRoles.Viewer:
                    if (userRole.Equals(UserRoles.Viewer)
                        || userRole.Equals(UserRoles.Administrator)
                        || userRole.Equals(UserRoles.Owner))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                    break;

                case UserRoles.Administrator:
                    if (userRole.Equals(UserRoles.Administrator)
                        || userRole.Equals(UserRoles.Owner))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                    break;

                case UserRoles.Owner:
                    if (userRole.Equals(UserRoles.Owner))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                    break;
            }
        }
    }
}
