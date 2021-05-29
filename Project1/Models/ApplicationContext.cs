using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Models
{
    public class ApplicationContext: IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>,ApplicationUserRole, IdentityUserLogin<string>,IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> opts) : base(opts)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId);
            });
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<ApplicationRole> ApplicationRoles { get; set; }

        public DbSet<FileManagement> FileManagement { get; set; }

        public DbSet<TranslationActivity> TranslationActivity { get; set; }

        public DbSet<DefaultLanguages> DefaultLanguages { get; set; }

        public DbSet<SupportLangagues> SupportLangagues { get; set; }

        public DbSet<SupportLangagueVersion> SupportLangagueVersion { get; set; }
    }

}
