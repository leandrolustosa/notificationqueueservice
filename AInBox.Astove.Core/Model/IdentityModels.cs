using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace AInBox.Astove.Core.Model
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser<int, UserLoginIntPk, UserRoleIntPk, UserClaimIntPk>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, RoleIntPk, int, UserLoginIntPk, UserRoleIntPk, UserClaimIntPk>
    {
        public ApplicationDbContext()
            : base("AstoveContext")
        {
        }

        public static ApplicationDbContext Create()
        {
            var context = new ApplicationDbContext();
            context.Database.CreateIfNotExists();
            return context;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, AInBox.Astove.Core.Migrations.Configuration>());

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RoleIntPk>()
               .Property(c => c.Name)
               .HasMaxLength(128)
               .IsRequired();
            modelBuilder.Entity<ApplicationUser>()
               .ToTable("Custom_AspNetUsers")
               .Property(c => c.UserName)
               .HasMaxLength(128)
               .IsRequired();
            modelBuilder.Entity<UserLoginIntPk>().ToTable("Users");
            modelBuilder.Entity<RoleIntPk>().ToTable("Roles");
            modelBuilder.Entity<UserRoleIntPk>().ToTable("UserRoles");
        }
    }

    public class UserLoginIntPk : IdentityUserLogin<int>
    { }

    public class UserRoleIntPk : IdentityUserRole<int>
    { }

    public class UserClaimIntPk : IdentityUserClaim<int>
    { }

    public class RoleIntPk : IdentityRole<int, UserRoleIntPk>
    {
        public RoleIntPk() { }
        public RoleIntPk(string name) { Name = name; }
    }

    public class UserStoreIntPk : UserStore<ApplicationUser, RoleIntPk, int,
    UserLoginIntPk, UserRoleIntPk, UserClaimIntPk>
    {
        public UserStoreIntPk(ApplicationDbContext context)
          : base(context)
        {
        }
    }

    public class RoleStoreIntPk : RoleStore<RoleIntPk, int, UserRoleIntPk>
    {
        public RoleStoreIntPk(ApplicationDbContext context)
           : base(context)
        {
        }
    }
}