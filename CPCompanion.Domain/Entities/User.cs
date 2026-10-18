using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CPCompanion.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string IdentityUserId { get; set; } = default!;
        public string CodeforcesHandle { get; set; } = default!;
        public int CurrentRating { get; set; }
        public int MaxRating { get; set; }
        public DateTime? LastSyncedAt { get; set; }
        public ICollection<Goal> Goals { get; set; } = new List<Goal>();
        public ICollection<ProblemLog> ProblemLogs { get; set; } = new List<ProblemLog>();
        public ICollection<SyncLog> SyncLogs { get; set; } = new List<SyncLog>();

    }
}
