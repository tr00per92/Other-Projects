namespace AuthorshipComparer.Data
{
    using System.Data.Entity;
    using System.Linq;
    using Common;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class DatabaseInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                var adminRole = new IdentityRole(GlobalConstants.AdminRole);
                context.Roles.Add(adminRole);

                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var admin = new ApplicationUser { UserName = "admin@admin.com", Email = "admin@admin.com" };
                userManager.Create(admin, "admin007");
                userManager.AddToRole(admin.Id, GlobalConstants.AdminRole);

                var user = new ApplicationUser { UserName = "user@user.com", Email = "user@user.com" };
                userManager.Create(user, "user007");
            }
        }
    }
}
