using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.DTOs.User
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string CodeforcesHandle { get; set; } = default!;
        public int CurrentRating { get; set; }
        public int MaxRating { get; set; }
        public DateTime? LastSyncedAt { get; set; }
    }
}
