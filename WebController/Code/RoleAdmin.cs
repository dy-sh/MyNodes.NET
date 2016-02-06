using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;

namespace MyNetSensors.WebController.Code
{
    public class RoleAdmin : AuthorizationHandler<RoleAdmin>, IAuthorizationRequirement
    {
        protected override void Handle(AuthorizationContext context, RoleAdmin requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.Role))
            {
                context.Fail();
                return;
            }

            string role = context.User.FindFirst(c => c.Type == ClaimTypes.Role).Value;
           
            if (role == "Admin")
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}

