using AutoMapper;
using CPCompanion.Application.DTOs.Goal;
using CPCompanion.Application.Exceptions;
using CPCompanion.Application.Interfaces;
using CPCompanion.Domain.Entities;
using CPCompanion.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Infrastructure.Services
{
    public class GoalService : IGoalService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GoalService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GoalResponseDto> CreateAsync(Guid userId, CreateGoalDto dto)
        {
            var goal = _mapper.Map<Goal>(dto);
            goal.Id = Guid.NewGuid();
            goal.UserId = userId;
            goal.IsCompleted = false;
            goal.CreatedAt = DateTime.UtcNow;
            goal.CurrentCount = 0;
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();

            return _mapper.Map<GoalResponseDto>(goal);
        }

        public async Task DeleteAsync(Guid userId, Guid goalId)
        {
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);

            if(goal == null)
            {
                throw new NotFoundException($"Goal with ID {goalId} not found for user {userId}");
            }

            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();

        }

        public async Task<List<GoalResponseDto>> GetAllForUserAsync(Guid userId)
        {
            var goals = await _context.Goals.AsNoTracking().Where(g => g.UserId == userId).ToListAsync();

            return _mapper.Map<List<GoalResponseDto>>(goals);
        }

        public async Task<GoalResponseDto> GetByIdAsync(Guid userId, Guid goalId)
        {
            var goal = await _context.Goals.AsNoTracking().FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);

            if(goal == null)
            {
                throw new NotFoundException($"Goal with ID {goalId} not found for user {userId}");
            }

            return _mapper.Map<GoalResponseDto>(goal);
        }

        public async Task<GoalResponseDto> UpdateAsync(Guid userId, Guid goalId, UpdateGoalDto dto)
        {
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);

            if (goal == null)
            {
                throw new NotFoundException($"Goal with ID {goalId} not found for user {userId}");
            }


            _mapper.Map(dto, goal);

            var problemLogs = await _context.ProblemLogs
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var count = CountMatchingProblems(goal, problemLogs);

            goal.CurrentCount = count;
            goal.IsCompleted = goal.CurrentCount >= goal.TargetCount;

            await _context.SaveChangesAsync();

            return _mapper.Map<GoalResponseDto>(goal);
        }
        public async Task UpdateGoalsAsync(Guid userId, List<ProblemLog> newProblemLogs ,
    CancellationToken cancellationToken = default)
        {
            var goals = await _context.Goals.Where(g => g.UserId == userId && !g.IsCompleted).ToListAsync(cancellationToken);
            foreach(var goal in goals)
            {
                var count = CountMatchingProblems(goal, newProblemLogs);

                goal.CurrentCount += count;
                goal.IsCompleted = goal.CurrentCount >= goal.TargetCount;

            }
            
        }
        private int CountMatchingProblems(
                    Goal goal,
                    IEnumerable<ProblemLog> problemLogs)
        {
            var targetTag = goal.TargetTag?.Trim();

            return problemLogs.Count(pl =>
                pl.SolvedAt >= goal.CreatedAt &&
                (!goal.Deadline.HasValue ||
                pl.SolvedAt <= goal.Deadline.Value) &&
                (
                    string.IsNullOrWhiteSpace(targetTag) ||
                    pl.Tags
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim())
                        .Contains(
                            targetTag,
                            StringComparer.OrdinalIgnoreCase)
                ));
        }
    }
}
