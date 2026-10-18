using CPCompanion.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasMany(u => u.ProblemLogs)
                   .WithOne(pl => pl.User)
                   .HasForeignKey(pl => pl.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Goals)
                   .WithOne(g => g.User)
                   .HasForeignKey(g => g.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(u => u.SyncLogs)
                   .WithOne()
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
