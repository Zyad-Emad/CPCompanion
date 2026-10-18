using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.DTOs.Stats
{
    public class DashboardStatsDto
    {
        public int TotalSolved { get; set; }
        public List<TagStatDto> TagBreakdown { get; set; } = new List<TagStatDto>();
        public StreakDto Streak { get; set; } = new StreakDto();
        public int CurrentRating { get; set; }
        public int MaxRating { get; set; }
    }
}
