using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace AspNetCore_WebApi_DevIO.Extensions
{
    public class AspNetUser : IUser
    {
        // This class is from AspNetCore, so it's not a good practice to inject it in other layers as you're generating some coupling with AspNetCore
        private readonly IHttpContextAccessor Accessor;

        public AspNetUser(IHttpContextAccessor accessor)
        {
            Accessor = accessor;
        }

        public string Name => Accessor.HttpContext.User.Identity.Name;

        public Guid GetUserId() => IsAuthenticated() ? Guid.Parse(Accessor.HttpContext.User.GetUserId()) : Guid.Empty;

        public string GetUserEmail() => IsAuthenticated() ? Accessor.HttpContext.User.GetUserEmail() : "";

        public bool IsAuthenticated() => Accessor.HttpContext.User.Identity.IsAuthenticated;

        public bool IsInRole(string role) => Accessor.HttpContext.User.IsInRole(role);

        public IEnumerable<Claim> GetClaimsIdentity() => Accessor.HttpContext.User.Claims;
    }

    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentException(nameof(principal));
            }
            var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            var id = claim?.Value;

            return id;
        }

        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentException(nameof(principal));
            }
            var claim = principal.FindFirst(ClaimTypes.Email);
            var email = claim?.Value;

            return email;
        }
    }
}
