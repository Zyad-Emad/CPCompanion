using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CPCompanion.Infrastructure.ExternalServices
{
    public class CodeforcesApiResponse
    {
        public string Status { get; set; } = default!;

        public List<CodeforcesSubmission> Result { get; set; } = new();
    }
    public class CodeforcesSubmission
    {
        public CodeforcesProblem Problem { get; set; } = default!;

        public string? Verdict { get; set; }

        public long CreationTimeSeconds { get; set; }
    }
    public class CodeforcesProblem
    {
        public int ContestId { get; set; }

        public string Index { get; set; } = default!;

        public string Name { get; set; } = default!;

        public List<string> Tags { get; set; } = new();

        public int? Rating { get; set; }
    }
    public class CodeforcesUserInfoResponse
    {
        public string Status { get; set; } = default!;
        public List<CodeforcesUserInfo> Result { get; set; } = new();

    }
    public class CodeforcesUserInfo
    {
        public string Handle { get; set; } = default!;
        public int Rating { get; set; }
        public int MaxRating { get; set; }
    }
}
