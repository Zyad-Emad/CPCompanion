using CPCompanion.Application.DTOs.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.Interfaces
{
    public interface ICodeforcesApiService
    {
        Task<List<CodeforcesSubmissionDto>> GetSolvedProblemsAsync(string handle, CancellationToken cancellationToken = default);
        Task<CodeforcesUserRatingDto> GetUserRatingAsync(string handle, CancellationToken cancellationToken = default);
    }
}
