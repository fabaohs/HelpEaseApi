using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpEaseApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelpEaseApi.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Models.Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            List<IdentityRole> roles = new List<IdentityRole>{
                new IdentityRole{Name = "Admin", NormalizedName = "ADMIN"},
                new IdentityRole{Name = "Manager", NormalizedName = "MANAGER"},
                new IdentityRole{Name = "User", NormalizedName = "USER"},
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}