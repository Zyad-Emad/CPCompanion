using CPCompanion.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CPCompanion.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class StatsController : BaseController
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = GetUserId();
            var result = await _statsService.GetDashboardStatsAsync(userId);
            return Ok(result);
        }
    }
}
