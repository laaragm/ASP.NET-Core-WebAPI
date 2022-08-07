using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace AspNetCore_WebApi_DevIO.Extensions
{
    public class CustomAuthorization
    {
        public static bool ValidateUserClaims(HttpContext context, string claimName, string claimValue)
            => UserIsAuthenticated(context) && UserHasClaim(context, claimName, claimValue);

        private static bool UserIsAuthenticated(HttpContext context) => context.User.Identity.IsAuthenticated;

        private static bool UserHasClaim(HttpContext context, string claimName, string claimValue) 
            => context.User.Claims.Any(c => c.Type == claimName && c.Value.Contains(claimValue));
    }

    public class ClaimsAuthorizeAttribute : TypeFilterAttribute
    {
        public ClaimsAuthorizeAttribute(string claimName, string claimValue) : base(typeof(RequirementClaimFilter))
        {
            Arguments = new object[] { new Claim(claimName, claimValue) };
        }
    }

    // Replaces standard Auth
    public class RequirementClaimFilter : IAuthorizationFilter
    {
        private readonly Claim Claim;

        public RequirementClaimFilter(Claim claim)
        {
            Claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                context.Result = new StatusCodeResult(401);
                return;
            }

            if (!CustomAuthorization.ValidateUserClaims(context.HttpContext, Claim.Type, Claim.Value))
            {
                context.Result = new StatusCodeResult(403); // Forbidden
            }
        }
    }
}
