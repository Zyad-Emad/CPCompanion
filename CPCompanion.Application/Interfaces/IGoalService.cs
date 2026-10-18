using CPCompanion.Application.DTOs.Goal;
using CPCompanion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.Interfaces
{
    public interface IGoalService
    {
        Task<GoalResponseDto> CreateAsync(Guid userId, CreateGoalDto dto);

        Task<List<GoalResponseDto>> GetAllForUserAsync(Guid userId);
        Task<GoalResponseDto> GetByIdAsync(Guid userId, Guid goalId);
        Task<GoalResponseDto> UpdateAsync(Guid userId, Guid goalId, UpdateGoalDto dto);
        Task DeleteAsync(Guid userId, Guid goalId);
        Task UpdateGoalsAsync(Guid userId, List<ProblemLog> newProblemLogs ,
    CancellationToken cancellationToken = default);

    }
}
