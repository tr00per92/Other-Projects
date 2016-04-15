namespace AuthorshipComparer.Web.Helpers
{
    using System.Web.Mvc;
    using Common;

    public class AuthorizeAdminAttribute : AuthorizeAttribute
    {
        public AuthorizeAdminAttribute()
        {
            this.Roles = GlobalConstants.AdminRole;
        }
    }
}
