using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Domain.Entities
{
    public class Goal
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string? TargetTag { get; set; }
        public int TargetCount { get; set; }
        public int CurrentCount { get; set; }
        public DateTime? Deadline { get; set; }
        public bool IsCompleted { get; set; }
        public Guid UserId { get; set; } = default!;
        public User User { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
