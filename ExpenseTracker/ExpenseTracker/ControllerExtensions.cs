// Copyright 2016 David Straw

using System.Security.Claims;
using System.Web.Http;

namespace ExpenseTracker
{
    static class ControllerExtensions
    {
        public static string GetCurrentUserSid(this ApiController controller)
        {
            // Get the SID of the current user.
            var claimsPrincipal = controller.User as ClaimsPrincipal;
            return claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GetCurrentUserGivenName(this ApiController controller)
        {
            var claimsPrincipal = controller.User as ClaimsPrincipal;
            return claimsPrincipal?.FindFirst(ClaimTypes.GivenName)?.Value;
        }
    }
}