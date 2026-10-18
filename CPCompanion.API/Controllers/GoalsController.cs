using CPCompanion.API.Filters;
using CPCompanion.Application.DTOs.Goal;
using CPCompanion.Application.Exceptions;
using CPCompanion.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CPCompanion.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class GoalsController : BaseController
    {
        private readonly IGoalService _goalService;

        public GoalsController(
            IGoalService goalService)
        {
            _goalService = goalService;
        }
        [ServiceFilter(typeof(ValidationFilter))]
        [HttpPost]
        public async Task<IActionResult> Create(CreateGoalDto dto)
        {
            var userId = GetUserId();
            var result = await _goalService.CreateAsync(userId, dto);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var result = await _goalService.GetAllForUserAsync(userId);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = GetUserId();
            var result = await _goalService.GetByIdAsync(userId, id);
            return Ok(result);
        }
        [ServiceFilter(typeof(ValidationFilter))]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateGoalDto dto)
        {
            var userId = GetUserId();
            var result = await _goalService.UpdateAsync(userId, id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            await _goalService.DeleteAsync(userId, id);
            return NoContent();
        }
        
    }
}
