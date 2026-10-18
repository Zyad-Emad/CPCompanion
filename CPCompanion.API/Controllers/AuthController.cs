using CPCompanion.API.Filters;
using CPCompanion.Application.DTOs.Auth;
using CPCompanion.Application.DTOs.User;
using CPCompanion.Application.Interfaces;
using CPCompanion.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CPCompanion.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ISyncService _syncService;


        public AuthController(IUserService userService, ISyncService syncService )
        {
            _userService = userService;
            _syncService = syncService;
        }

        [ServiceFilter(typeof(ValidationFilter))]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            var result = await _userService.RegisterAsync(dto);
            return Ok(result);
        }
        [ServiceFilter(typeof(ValidationFilter))]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _userService.LoginAsync(dto);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            var result = await _userService.GetProfileAsync(userId);
            return Ok(result);
        }

        [Authorize]
        [ServiceFilter(typeof(ValidationFilter))]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var userId = GetUserId();
            var result = await _userService.UpdateProfileAsync(userId, dto);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("sync-now")]
        public async Task<IActionResult> SyncNow()
        {
            var userId = GetUserId();
            await _syncService.SyncUserAsync(userId);
            return NoContent();
        }
    }
}