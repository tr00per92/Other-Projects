namespace AuthorshipComparer.Web.Helpers
{
    using System.Security.Principal;
    using Common;
    using Microsoft.AspNet.Identity;

    public static class Extensions
    {
        public static bool IsLoggedIn(this IPrincipal principal)
        {
            return principal.Identity.IsAuthenticated;
        }

        public static bool IsAdmin(this IPrincipal principal)
        {
            return principal.IsInRole(GlobalConstants.AdminRole);
        }

        public static string GetId(this IPrincipal principal)
        {
            return principal.Identity.GetUserId();
        }
    }
}
