using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.DTOs.Goal
{
    public class CreateGoalDto
    {
        public string Title { get; set; } = default!;
        public string? TargetTag { get; set; }
        public int TargetCount { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
