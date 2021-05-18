using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Models
{
    public class ApplicationContext: DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> opts) : base(opts)
        {
        }

        public DbSet<FileManagement> FileManagement { get; set; }

        public DbSet<ExportFiles> ExportFiles { get; set; }
    }
}
