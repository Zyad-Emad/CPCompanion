using CPCompanion.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Domain.Entities
{
    public class SyncLog
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime SyncedAt { get; set; }
        public int NewProblemsCount { get; set; }
        public SyncStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
