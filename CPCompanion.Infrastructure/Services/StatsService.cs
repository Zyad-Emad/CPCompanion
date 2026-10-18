using CPCompanion.Application.DTOs.Stats;
using CPCompanion.Application.Exceptions;
using CPCompanion.Application.Interfaces;
using CPCompanion.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Infrastructure.Services
{
    public class StatsService : IStatsService
    {
        private readonly AppDbContext _context;

        public StatsService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<DashboardStatsDto> GetDashboardStatsAsync(Guid userId)
        {
            var user = await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null)
            {
                throw new NotFoundException("User not found.");
            }
            var problemLogs = await _context.ProblemLogs
               .AsNoTracking()
               .Where(p => p.UserId == userId)
               .ToListAsync();

            var tagBreakdown = problemLogs
               .SelectMany(p => p.Tags.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
               .GroupBy(tag => tag)
               .Select(g => new TagStatDto { Tag = g.Key, SolvedCount = g.Count() })
               .OrderByDescending(t => t.SolvedCount)
               .ToList();

            var streak = CalculateStreak(problemLogs.Select(p => p.SolvedAt).ToList());

            return new DashboardStatsDto
            {
                TotalSolved = problemLogs.Count,
                TagBreakdown = tagBreakdown,
                Streak = streak,
                CurrentRating = user.CurrentRating,
                MaxRating = user.MaxRating
            };
        }
        private StreakDto CalculateStreak(List<DateTime> solvedDates)
        {
            if (!solvedDates.Any())
            {
                return new StreakDto { CurrentStreak = 0, LongestStreak = 0 };
            }

            var distinctDays = solvedDates
                .Select(d => DateOnly.FromDateTime(d))
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();


            int currentStreak = 0;
            int longestStreak = 0;
            int tempStreak = 1;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (distinctDays[0] == today || distinctDays[0] == today.AddDays(-1))
            {
                currentStreak = 1;
                for (int i = 0; i < distinctDays.Count - 1; i++)
                {
                    if (distinctDays[i].AddDays(-1) == distinctDays[i + 1])
                    {
                        currentStreak++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            for (int i = 0; i < distinctDays.Count - 1; i++)
            {
                if (distinctDays[i].AddDays(-1) == distinctDays[i + 1])
                {
                    tempStreak++;
                }
                else
                {
                    longestStreak = Math.Max(longestStreak, tempStreak);
                    tempStreak = 1;
                }
            }
            longestStreak = Math.Max(longestStreak, tempStreak);
            return new StreakDto { CurrentStreak = currentStreak, LongestStreak = longestStreak };
        }
    }
}
