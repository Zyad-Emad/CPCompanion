using CPCompanion.Domain.Entities;
using CPCompanion.Infrastructure.Identity;
using CPCompanion.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
            
        }
        public DbSet<User> UserProfiles { get; set; }
        public DbSet<ProblemLog> ProblemLogs { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<SyncLog> SyncLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new ProblemLogConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
