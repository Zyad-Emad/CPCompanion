using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Domain.Entities
{
    public class ProblemLog
    {
        public Guid Id { get; set; }
        public string ProblemId { get; set; } = default!;
        public string ProblemName { get; set; } = default!;
        public string Tags { get; set; } = default!;
        public int? Difficulty { get; set; }
        public DateTime SolvedAt { get; set; }
        public Guid UserId { get; set; } = default!;
        public User User { get; set; } = default!;
    }
}
