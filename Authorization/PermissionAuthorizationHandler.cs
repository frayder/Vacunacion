using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Highdmin.Services;

namespace Highdmin.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Resource { get; }
        public string Action { get; }

        public PermissionRequirement(string resource, string action)
        {
            Resource = resource;
            Action = action;
        }
    }

    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly AuthorizationService _authorizationService;

        public PermissionAuthorizationHandler(AuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
            {
                return;
            }

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return;
            }

            var hasPermission = await _authorizationService.HasPermissionAsync(userId, requirement.Resource, requirement.Action);
            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}