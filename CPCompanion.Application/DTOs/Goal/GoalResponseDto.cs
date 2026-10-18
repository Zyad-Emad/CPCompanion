using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.DTOs.Goal
{
    public class GoalResponseDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? TargetTag { get; set; }
        public int TargetCount { get; set; }
        public int CurrentCount { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsCompleted { get; set; }
    }
}
