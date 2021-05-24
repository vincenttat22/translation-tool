using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Models
{
    public class ApplicationContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> opts) : base(opts)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<FileManagement> FileManagement { get; set; }

        public DbSet<TranslationActivity> TranslationActivity { get; set; }

        public DbSet<DefaultLanguages> DefaultLanguages { get; set; }
    }
}
