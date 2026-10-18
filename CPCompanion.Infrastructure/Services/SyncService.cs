using CPCompanion.Application.Exceptions;
using CPCompanion.Application.Interfaces;
using CPCompanion.Domain.Entities;
using CPCompanion.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Infrastructure.Services
{
    public class SyncService : ISyncService
    {
        private readonly AppDbContext _context;
        private readonly ICodeforcesApiService _codeforcesApi;
        private readonly IGoalService _goalService;
        private readonly ILogger<SyncService> _logger;

        public SyncService(AppDbContext context, ICodeforcesApiService codeforcesApi, ILogger<SyncService> logger, IGoalService goalService)
        {
            _context = context;
            _codeforcesApi = codeforcesApi;
            _logger = logger;
            _goalService = goalService;
        }

        public async Task SyncUserAsync(Guid userId,
    CancellationToken cancellationToken = default)
        {
            var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == userId , cancellationToken);
            if (user is null)
            {
                throw new NotFoundException("User not found.");
            }

            var ratingTask = _codeforcesApi.GetUserRatingAsync(user.CodeforcesHandle , cancellationToken);
            var solvedProblemsTask = _codeforcesApi.GetSolvedProblemsAsync(user.CodeforcesHandle , cancellationToken);

            await Task.WhenAll(ratingTask, solvedProblemsTask);



            var ratingInfo = await ratingTask;
            if (ratingInfo.Exists)
            {
                user.CurrentRating = ratingInfo.CurrentRating;
                user.MaxRating = ratingInfo.MaxRating;
            }

            var solvedProblems = await solvedProblemsTask;
            var existingProblemLogs = await _context.ProblemLogs.Where(p => p.UserId == user.Id).ToListAsync(cancellationToken);

            var uniqueSolvedProblems = solvedProblems
                .GroupBy(sp => sp.ProblemId)
                .Select(g => g.OrderBy(sp => sp.SolvedAt).First())
                .ToList();

            var newProblemLogs = uniqueSolvedProblems
                .Where(sp => !existingProblemLogs.Any(ep => ep.ProblemId == sp.ProblemId))
                .Select(sp => new ProblemLog
                {
                    UserId = user.Id,
                    ProblemId = sp.ProblemId,
                    ProblemName = sp.ProblemName!,
                    Difficulty = sp.Difficulty,
                    SolvedAt = sp.SolvedAt,
                    Tags = string.Join(", ", sp.Tags)
                })
                .ToList();

            if (newProblemLogs.Any())
            {
                _context.ProblemLogs.AddRange(newProblemLogs);

                await _goalService.UpdateGoalsAsync(user.Id, newProblemLogs , cancellationToken);
            }

           
            user.LastSyncedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

       
    }
}
