using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CLIP.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string EmpID { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<CompetencyModule> CompetencyModules { get; set; }
        public DbSet<UserCompetency> UserCompetencies { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure UserCompetency relationships
            modelBuilder.Entity<UserCompetency>()
                .HasRequired(uc => uc.User)
                .WithMany()
                .HasForeignKey(uc => uc.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserCompetency>()
                .HasRequired(uc => uc.CompetencyModule)
                .WithMany(cm => cm.UserCompetencies)
                .HasForeignKey(uc => uc.CompetencyModuleId)
                .WillCascadeOnDelete(false);
        }
    }
}