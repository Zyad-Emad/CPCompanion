using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.DTOs.External
{
    public class CodeforcesSubmissionDto
    {
        public string ProblemId { get; set; } = default!;
        public string? ProblemName { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public int? Difficulty { get; set; }
        public DateTime SolvedAt { get; set; }
    }
}
