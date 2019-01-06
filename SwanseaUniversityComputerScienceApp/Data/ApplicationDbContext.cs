using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SwanseaUniversityComputerScienceApp.Models;

namespace SwanseaUniversityComputerScienceApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        // All the additional databases for the application
        public DbSet<SwanseaUniversityComputerScienceApp.Models.Post> Post { get; set; }
        public DbSet<SwanseaUniversityComputerScienceApp.Models.Comment> Comment { get; set; }
        public DbSet<SwanseaUniversityComputerScienceApp.Models.Module> Modules { get; set; }
    }
}
