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
    public class ProblemLogConfiguration
    : IEntityTypeConfiguration<ProblemLog>
    {
        public void Configure(EntityTypeBuilder<ProblemLog> builder)
        {
            builder.HasIndex(p => new
            {
                p.UserId,
                p.ProblemId
            }).IsUnique();
        }
    }
}
