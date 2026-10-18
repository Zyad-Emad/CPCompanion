using CPCompanion.Application.DTOs.External;
using CPCompanion.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CPCompanion.Infrastructure.ExternalServices
{
    public class CodeforcesApiService : ICodeforcesApiService
    {
        private readonly HttpClient _httpClient;
        public CodeforcesApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<CodeforcesSubmissionDto>> GetSolvedProblemsAsync(string handle,
    CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetStringAsync($"user.status?handle={handle}", cancellationToken);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<CodeforcesApiResponse>(response, options);

            if(data == null || data.Result == null)
            {
                 throw new Exception("Failed to fetch data from Codeforces API.");
            }

            var VerifiedSubmissions = data.Result.Where(r => r.Verdict == "OK").ToList();

            List<CodeforcesSubmissionDto> codeforcesSubmissionDtos = new List<CodeforcesSubmissionDto>();

            foreach(var submission in VerifiedSubmissions)
            {
                codeforcesSubmissionDtos.Add(new CodeforcesSubmissionDto
                {
                    Difficulty = submission.Problem.Rating,
                    ProblemId = $"{submission.Problem.ContestId}{submission.Problem.Index}",
                    ProblemName = submission.Problem.Name,
                    SolvedAt = DateTimeOffset.FromUnixTimeSeconds(submission.CreationTimeSeconds).UtcDateTime,
                    Tags = submission.Problem.Tags
                });
            }
            return codeforcesSubmissionDtos;
        }

        public async Task<CodeforcesUserRatingDto> GetUserRatingAsync(string handle,
    CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"user.info?handles={handle}", cancellationToken);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<CodeforcesUserInfoResponse>(response, options);

                if (data == null || data.Result == null || data.Result.Count == 0)
                {
                    return new CodeforcesUserRatingDto { Exists = false };
                }

                var userInfo = data.Result[0]; 

                return new CodeforcesUserRatingDto
                {
                    Exists = true,
                    CurrentRating = userInfo.Rating,
                    MaxRating = userInfo.MaxRating
                };
            }
            catch (HttpRequestException)
            {
                return new CodeforcesUserRatingDto { Exists = false };
            }
        }

        
    }
}
